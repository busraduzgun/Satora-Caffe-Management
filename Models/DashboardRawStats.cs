using System.ComponentModel.DataAnnotations.Schema;

namespace SatoraCaffeRestaurantTracking.Models
{
    [NotMapped]
    public class DashboardRawStats
    {
        public decimal GunlukCiro { get; set; }
        public int GunlukSiparis { get; set; }
        public decimal TahminiStokMaliyeti { get; set; }
        public int KritikStokSayisi { get; set; }
    }
}