namespace ChurchRota.Library.Data;

public class Schedule
{
    public int ScheduleId { get; set; }
    public int PersonId { get; set; }
    public int RoleId { get; set; }
    public DateTime ServiceDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Person Person { get; set; } = null!;
    public Role Role { get; set; } = null!;
}