using System;

namespace SatoraCaffeRestaurantTracking.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public decimal Amount { get; set; }

        public int PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }

       
        public DateTime PaymentDate { get; set; }

        
        public bool Status { get; set; }

        public int? UserId { get; set; }    // Ödemeyi Alan Personel
    }
}