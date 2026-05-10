using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class Staff
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public bool Status { get; set; }
    public string? Email { get; set; }     
    public string? Password { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual StaffRole Role { get; set; } = null!;

    public virtual ICollection<StaffLog> StaffLogs { get; set; } = new List<StaffLog>();

    public virtual User User { get; set; } = null!;
}
