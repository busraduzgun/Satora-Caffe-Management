using System;
using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            Payments = new List<Payment>();
            DeliveryOrders = new List<DeliveryOrder>();
        }

        public int Id { get; set; }

        // --- Masa Siparişleri İçin ---
        public int? TableId { get; set; }
        public virtual RestaurantTable Table { get; set; }

        // --- Müşteri / Paket Siparişleri İçin ---
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? CloseDate { get; set; }

        public bool Status { get; set; } // true: Açık, false: Kapalı

        public int ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; } 

        public int? StaffId { get; set; }
        public virtual Staff Staff { get; set; } 

        // İlişkiler
        public virtual List<OrderDetail> OrderDetails { get; set; }

        
        public virtual List<Payment> Payments { get; set; }
        public virtual List<DeliveryOrder> DeliveryOrders { get; set; }
    }
}