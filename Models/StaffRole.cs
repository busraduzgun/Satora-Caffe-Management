using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class StaffRole
{
    public int Id { get; set; }

    public string RoleNmae { get; set; } = null!;

    public string? Explanation { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
