using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;

namespace ChurchRota.SwapFunction.Services;

public class ReaderMatchingService : IReaderMatchingService
{
    private readonly IPeopleRepository _peopleRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public ReaderMatchingService(
        IPeopleRepository peopleRepository,
        IAvailabilityRepository availabilityRepository,
        IScheduleRepository scheduleRepository)
    {
        _peopleRepository = peopleRepository;
        _availabilityRepository = availabilityRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ReplacementReaderDto?> FindReplacementAsync(string scheduleId, string roleId, DateTime date)
    {
        // Get all people with the required role
        var candidates = await _peopleRepository.GetByRoleAsync(roleId);
        
        var scoredCandidates = new List<(PersonTableEntity person, int score)>();
        
        foreach (var candidate in candidates)
        {
            // Check if they're available on this date
            var availability = await _availabilityRepository.GetAsync(candidate.PersonId, date);
            
            // If no availability record exists, assume available
            // If record exists and IsAvailable is false, skip this candidate
            if (availability != null && !availability.IsAvailable)
            {
                continue;
            }
            
            // Check if they're already scheduled on this date
            var existingSchedule = await _scheduleRepository.GetByDateRangeAsync(date, date);
            if (existingSchedule.Any(s => s.PersonId == candidate.PersonId))
            {
                continue; // Skip if already scheduled
            }
            
            // Calculate fairness score (lower is better - fewer assignments in the last 3 months)
            var threeMonthsAgo = date.AddMonths(-3);
            var assignmentCount = await _scheduleRepository.GetPersonAssignmentCountAsync(
                candidate.PersonId, 
                threeMonthsAgo, 
                date);
            
            scoredCandidates.Add((candidate, assignmentCount));
        }
        
        // Select the candidate with the lowest assignment count (most fair)
        var bestCandidate = scoredCandidates
            .OrderBy(c => c.score)
            .FirstOrDefault();
        
        if (bestCandidate.person == null)
        {
            return null;
        }
        
        return new ReplacementReaderDto
        {
            PersonId = bestCandidate.person.PersonId,
            Name = bestCandidate.person.Name,
            Phone = bestCandidate.person.Phone,
            Email = bestCandidate.person.Email
        };
    }

    public async Task<List<SuggestionDto>> FindAlternativesAsync(string roleId, DateTime date)
    {
        // Get all people with the required role
        var candidates = await _peopleRepository.GetByRoleAsync(roleId);
        
        var suggestions = new List<SuggestionDto>();
        
        foreach (var candidate in candidates)
        {
            // Check their availability in the next 30 days
            var futureAvailability = await _availabilityRepository.GetByPersonAsync(
                candidate.PersonId, 
                date, 
                date.AddDays(30));
            
            var nextAvailableDate = futureAvailability
                .Where(a => a.IsAvailable)
                .OrderBy(a => a.Date)
                .FirstOrDefault();
            
            if (nextAvailableDate != null)
            {
                suggestions.Add(new SuggestionDto
                {
                    PersonId = candidate.PersonId,
                    Name = candidate.Name,
                    AvailableFrom = nextAvailableDate.Date
                });
            }
        }
        
        return suggestions.OrderBy(s => s.AvailableFrom).ToList();
    }
}
