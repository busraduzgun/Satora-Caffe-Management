using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class StaffLog
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public string Operation { get; set; } = null!;

    public DateTime LogDate { get; set; }

    public bool Status { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
