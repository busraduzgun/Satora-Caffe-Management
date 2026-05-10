using System.Collections.Generic;

namespace SatoraCaffeRestaurantTracking.Models
{
    // Bu sınıf veritabanında tablo değildir.
    // Sadece Controller'dan View'e veri taşıyan "Servis Tepsisidir".
    public class DashboardViewModel
    {
        // 1. Günlük Operasyon Verileri (Kartlar için)
        public decimal GunlukCiro { get; set; }
        public int MusteriSayisi { get; set; }
        public int AcikSiparis { get; set; }
        public int GecikenSiparis { get; set; }
        public int IptalSiparis { get; set; }

        // 2. Stok Listesi (Tablo için)
        public List<Product> KritikStoklar { get; set; }

        // 3. En Çok Satanlar Grafiği (Pasta Grafik için)
        // Örn: { "Latte", 50 }, { "Çay", 30 }
        public Dictionary<string, int> EnCokSatilanlar { get; set; }

        // 4. Saatlik Yoğunluk Grafiği (Sütun Grafik için) 
        // Örn: { "09:00", 5 }, { "10:00", 12 }
        public Dictionary<string, int> SaatlikYogunluk { get; set; }
    }
}