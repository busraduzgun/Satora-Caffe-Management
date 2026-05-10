using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class ServiceType
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Explanation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();
}
