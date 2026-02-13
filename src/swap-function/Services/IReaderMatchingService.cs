using ChurchRota.SwapFunction.Models;

namespace ChurchRota.SwapFunction.Services;

public interface IReaderMatchingService
{
    Task<ReplacementReaderDto?> FindReplacementAsync(string scheduleId, string roleId, DateTime date);
    Task<List<SuggestionDto>> FindAlternativesAsync(string roleId, DateTime date);
}
