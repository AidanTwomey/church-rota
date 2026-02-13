namespace ChurchRota.SwapFunction.Models;

public class ScheduleTableEntity : Azure.Data.Tables.ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public Azure.ETag ETag { get; set; }

    // Schedule properties
    public string ScheduleId { get; set; } = default!;
    public DateTime Date { get; set; }
    public string? Solemnity { get; set; }
    public string PersonId { get; set; } = default!;
    public string PersonName { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
    public string Status { get; set; } = "Confirmed"; // Confirmed, NeedsCover, Covered
    public string? Notes { get; set; }
}
