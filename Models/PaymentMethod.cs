using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class PaymentMethod
{
    public int Id { get; set; }

    public string MethodName { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
