using System.ComponentModel.DataAnnotations.Schema;

namespace SatoraCaffeRestaurantTracking.Models
{
    [NotMapped]
    public class BestSellerRawStats
    {
        public string ProductName { get; set; }
        public int ToplamSatisAdeti { get; set; }
        public decimal ToplamCiro { get; set; }
    }
}