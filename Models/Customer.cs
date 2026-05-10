using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models;

public partial class Customer
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? Address { get; set; }

    public DateTime? FirstOrderDate { get; set; }

    public DateTime? LastOrderDate { get; set; }

    public int? OrderCount { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Messages> Messages { get; set; } = new List<Messages>();
}
