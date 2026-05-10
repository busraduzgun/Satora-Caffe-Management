using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SatoraCaffeRestaurantTracking.Models;

[Table("RestaurantTables")] 
public partial class RestaurantTable
{
    [Column("ID")] 
    public int Id { get; set; }

    [Column("ServiceTypeID")] 
    public int ServiceTypeId { get; set; }

    public int Capacity { get; set; }

    public byte Status { get; set; } 

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ServiceType ServiceType { get; set; } = null!;
}
