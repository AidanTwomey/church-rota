namespace ChurchRota.SwapFunction.Models;

public class PersonTableEntity : Azure.Data.Tables.ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public Azure.ETag ETag { get; set; }

    // Person properties
    public string PersonId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Roles { get; set; } = default!; // Comma-separated role IDs
}
