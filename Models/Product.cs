using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Stock { get; set; }

    public string? Image { get; set; }

    public bool Status { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
