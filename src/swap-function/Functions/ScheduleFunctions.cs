using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;

namespace ChurchRota.SwapFunction.Functions;

public class ScheduleFunctions
{
    private readonly ILogger<ScheduleFunctions> _logger;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPeopleRepository _peopleRepository;

    public ScheduleFunctions(
        ILogger<ScheduleFunctions> logger,
        IScheduleRepository scheduleRepository,
        IPeopleRepository peopleRepository)
    {
        _logger = logger;
        _scheduleRepository = scheduleRepository;
        _peopleRepository = peopleRepository;
    }

    [Function("GetSchedule")]
    public async Task<HttpResponseData> GetSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedule")] HttpRequestData req)
    {
        _logger.LogInformation("Getting schedule");

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var startDateStr = query["startDate"];
        var endDateStr = query["endDate"];

        if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("startDate and endDate query parameters are required");
            return badResponse;
        }

        if (!DateTime.TryParse(startDateStr, out var startDate) || 
            !DateTime.TryParse(endDateStr, out var endDate))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid date format");
            return badResponse;
        }

        var schedules = await _scheduleRepository.GetByDateRangeAsync(startDate, endDate);
        
        var dtos = schedules.Select(s => new ScheduleEntryDto
        {
            ScheduleId = s.ScheduleId,
            Date = s.Date,
            Solemnity = s.Solemnity,
            PersonId = s.PersonId,
            PersonName = s.PersonName,
            RoleId = s.RoleId,
            RoleName = s.RoleName,
            Status = s.Status,
            Notes = s.Notes
        }).ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(dtos);
        return response;
    }

    [Function("GetScheduleEntry")]
    public async Task<HttpResponseData> GetScheduleEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedule/{scheduleId}")] HttpRequestData req,
        string scheduleId)
    {
        _logger.LogInformation("Getting schedule entry {ScheduleId}", scheduleId);

        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        
        if (schedule == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var dto = new ScheduleEntryDto
        {
            ScheduleId = schedule.ScheduleId,
            Date = schedule.Date,
            Solemnity = schedule.Solemnity,
            PersonId = schedule.PersonId,
            PersonName = schedule.PersonName,
            RoleId = schedule.RoleId,
            RoleName = schedule.RoleName,
            Status = schedule.Status,
            Notes = schedule.Notes
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(dto);
        return response;
    }

    [Function("CreateScheduleEntry")]
    public async Task<HttpResponseData> CreateScheduleEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "schedule")] HttpRequestData req)
    {
        _logger.LogInformation("Creating schedule entry");

        var body = await req.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<CreateScheduleEntryDto>(body!, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (dto == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body");
            return badResponse;
        }

        // Get person details to populate the schedule entry
        var person = await _peopleRepository.GetByIdAsync(dto.PersonId);
        if (person == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Person {dto.PersonId} not found");
            return notFoundResponse;
        }

        var entity = new ScheduleTableEntity
        {
            ScheduleId = Guid.NewGuid().ToString(),
            Date = dto.Date,
            PersonId = dto.PersonId,
            PersonName = person.Name,
            RoleId = dto.RoleId,
            RoleName = "Reader", // TODO: Look up role name
            Status = "Confirmed",
            Notes = dto.Notes
        };

        var created = await _scheduleRepository.CreateAsync(entity);

        var resultDto = new ScheduleEntryDto
        {
            ScheduleId = created.ScheduleId,
            Date = created.Date,
            Solemnity = created.Solemnity,
            PersonId = created.PersonId,
            PersonName = created.PersonName,
            RoleId = created.RoleId,
            RoleName = created.RoleName,
            Status = created.Status,
            Notes = created.Notes
        };

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(resultDto);
        return response;
    }

    [Function("UpdateScheduleEntry")]
    public async Task<HttpResponseData> UpdateScheduleEntry(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "schedule/{scheduleId}")] HttpRequestData req,
        string scheduleId)
    {
        _logger.LogInformation("Updating schedule entry {ScheduleId}", scheduleId);

        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var body = await req.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<UpdateScheduleEntryDto>(body!, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (dto == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body");
            return badResponse;
        }

        if (!string.IsNullOrEmpty(dto.PersonId))
        {
            var person = await _peopleRepository.GetByIdAsync(dto.PersonId);
            if (person != null)
            {
                schedule.PersonId = dto.PersonId;
                schedule.PersonName = person.Name;
            }
        }

        if (!string.IsNullOrEmpty(dto.Status))
        {
            schedule.Status = dto.Status;
        }

        if (dto.Notes != null)
        {
            schedule.Notes = dto.Notes;
        }

        var updated = await _scheduleRepository.UpdateAsync(schedule);

        var resultDto = new ScheduleEntryDto
        {
            ScheduleId = updated.ScheduleId,
            Date = updated.Date,
            Solemnity = updated.Solemnity,
            PersonId = updated.PersonId,
            PersonName = updated.PersonName,
            RoleId = updated.RoleId,
            RoleName = updated.RoleName,
            Status = updated.Status,
            Notes = updated.Notes
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(resultDto);
        return response;
    }
}
