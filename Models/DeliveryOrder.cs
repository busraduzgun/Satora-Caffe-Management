using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class DeliveryOrder
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? Note { get; set; }

    public byte Status { get; set; }

    public virtual Order Order { get; set; } = null!;
}
