using System;

namespace ChurchRota.Library.Data;

public class PeopleRole
{
    // Composite key will be configured on PersonId + RoleId
    public int PersonId { get; set; }
    public int RoleId { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public Person? Person { get; set; }
    public Role? Role { get; set; }
}
