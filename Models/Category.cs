using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
