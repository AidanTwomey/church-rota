namespace ChurchRota.Library.Data;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}