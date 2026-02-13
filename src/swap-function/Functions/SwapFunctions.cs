using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;
using ChurchRota.SwapFunction.Services;

namespace ChurchRota.SwapFunction.Functions;

public class SwapFunctions
{
    private readonly ILogger<SwapFunctions> _logger;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IReaderMatchingService _matchingService;

    public SwapFunctions(
        ILogger<SwapFunctions> logger,
        IScheduleRepository scheduleRepository,
        IReaderMatchingService matchingService)
    {
        _logger = logger;
        _scheduleRepository = scheduleRepository;
        _matchingService = matchingService;
    }

    [Function("RequestSwap")]
    public async Task<HttpResponseData> RequestSwap(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "schedule/{scheduleId}")] HttpRequestData req,
        string scheduleId)
    {
        _logger.LogInformation("Processing swap request for schedule {ScheduleId}", scheduleId);

        // Check Content-Type header for custom media type
        var contentType = req.Headers.GetValues("Content-Type").FirstOrDefault();
        if (contentType != "application/vnd.church-rota.swap-request+json")
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid Content-Type. Expected: application/vnd.church-rota.swap-request+json");
            return badResponse;
        }

        // Get the schedule entry
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Schedule entry not found");
            return notFoundResponse;
        }

        // Parse the swap request (optional reason)
        var body = await req.ReadAsStringAsync();
        var swapRequest = JsonSerializer.Deserialize<SwapRequestDto>(body ?? "{}", new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Find a replacement reader
        var replacement = await _matchingService.FindReplacementAsync(
            scheduleId, 
            schedule.RoleId, 
            schedule.Date);

        HttpResponseData response;

        if (replacement != null)
        {
            // Update the schedule with the replacement
            var originalPersonId = schedule.PersonId;
            var originalPersonName = schedule.PersonName;
            
            schedule.PersonId = replacement.PersonId;
            schedule.PersonName = replacement.Name;
            schedule.Status = "Covered";
            schedule.Notes = string.IsNullOrEmpty(schedule.Notes) 
                ? $"Swap from {originalPersonName}. Reason: {swapRequest?.Reason ?? "Not specified"}"
                : schedule.Notes + $" | Swap from {originalPersonName}. Reason: {swapRequest?.Reason ?? "Not specified"}";

            await _scheduleRepository.UpdateAsync(schedule);

            var successDto = new SwapResponseDto
            {
                Success = true,
                SwapId = scheduleId,
                Replacement = replacement
            };

            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/vnd.church-rota.swap-response+json");
            await response.WriteAsJsonAsync(successDto);
            
            _logger.LogInformation("Swap successful: {OriginalPerson} -> {Replacement}", 
                originalPersonName, replacement.Name);
        }
        else
        {
            // No replacement found, provide suggestions
            var alternatives = await _matchingService.FindAlternativesAsync(
                schedule.RoleId, 
                schedule.Date);

            var failureDto = new SwapResponseDto
            {
                Success = false,
                Message = "No available replacement found",
                Suggestions = alternatives
            };

            response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/vnd.church-rota.swap-response+json");
            await response.WriteAsJsonAsync(failureDto);
            
            _logger.LogWarning("No replacement found for schedule {ScheduleId}", scheduleId);
        }

        return response;
    }
}
