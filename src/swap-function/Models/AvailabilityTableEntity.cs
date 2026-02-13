namespace ChurchRota.SwapFunction.Models;

public class AvailabilityTableEntity : Azure.Data.Tables.ITableEntity
{
    public string PartitionKey { get; set; } = default!; // PersonId
    public string RowKey { get; set; } = default!; // Date in yyyyMMdd format
    public DateTimeOffset? Timestamp { get; set; }
    public Azure.ETag ETag { get; set; }

    // Availability properties
    public string PersonId { get; set; } = default!;
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
}
