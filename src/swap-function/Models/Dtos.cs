namespace ChurchRota.SwapFunction.Models;

public class SwapRequestDto
{
    public string? Reason { get; set; }
}

public class SwapResponseDto
{
    public bool Success { get; set; }
    public string? SwapId { get; set; }
    public ReplacementReaderDto? Replacement { get; set; }
    public string? Message { get; set; }
    public List<SuggestionDto>? Suggestions { get; set; }
}

public class ReplacementReaderDto
{
    public string PersonId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class SuggestionDto
{
    public string PersonId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime? AvailableFrom { get; set; }
}

public class ScheduleEntryDto
{
    public string ScheduleId { get; set; } = default!;
    public DateTime Date { get; set; }
    public string? Solemnity { get; set; }
    public string PersonId { get; set; } = default!;
    public string PersonName { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }
}

public class CreateScheduleEntryDto
{
    public DateTime Date { get; set; }
    public string PersonId { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string? SolemnityId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateScheduleEntryDto
{
    public string? PersonId { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

public class AvailabilityEntryDto
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
}

public class SetAvailabilityDto
{
    public List<AvailabilityEntryDto> Dates { get; set; } = new();
}
