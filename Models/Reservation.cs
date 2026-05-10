using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int TableId { get; set; }

    public DateTime ReservationDate { get; set; }

    public int? GuestCount { get; set; }

    public string? Description { get; set; }

    public int? OrderId { get; set; }

    public byte Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual RestaurantTable Table { get; set; } = null!;
}
