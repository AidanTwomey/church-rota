using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;

namespace ChurchRota.SwapFunction.Functions;

public class AvailabilityFunctions
{
    private readonly ILogger<AvailabilityFunctions> _logger;
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityFunctions(
        ILogger<AvailabilityFunctions> logger,
        IAvailabilityRepository availabilityRepository)
    {
        _logger = logger;
        _availabilityRepository = availabilityRepository;
    }

    [Function("GetAvailability")]
    public async Task<HttpResponseData> GetAvailability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "availability/{personId}")] HttpRequestData req,
        string personId)
    {
        _logger.LogInformation("Getting availability for person {PersonId}", personId);

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var startDateStr = query["startDate"];
        var endDateStr = query["endDate"];

        DateTime? startDate = null;
        DateTime? endDate = null;

        if (!string.IsNullOrEmpty(startDateStr) && DateTime.TryParse(startDateStr, out var parsedStart))
        {
            startDate = parsedStart;
        }

        if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out var parsedEnd))
        {
            endDate = parsedEnd;
        }

        var availability = await _availabilityRepository.GetByPersonAsync(personId, startDate, endDate);
        
        var dtos = availability.Select(a => new AvailabilityEntryDto
        {
            Date = a.Date,
            IsAvailable = a.IsAvailable
        }).ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(dtos);
        return response;
    }

    [Function("SetAvailability")]
    public async Task<HttpResponseData> SetAvailability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "availability/{personId}")] HttpRequestData req,
        string personId)
    {
        _logger.LogInformation("Setting availability for person {PersonId}", personId);

        var body = await req.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<SetAvailabilityDto>(body!, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (dto == null || dto.Dates == null || !dto.Dates.Any())
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body");
            return badResponse;
        }

        var entities = dto.Dates.Select(d => new AvailabilityTableEntity
        {
            PersonId = personId,
            Date = d.Date,
            IsAvailable = d.IsAvailable
        }).ToList();

        await _availabilityRepository.SetBulkAsync(personId, entities);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Availability updated successfully");
        return response;
    }
}
