using System.ComponentModel.DataAnnotations.Schema;

namespace SatoraCaffeRestaurantTracking.Models
{
    // 1. DASHBOARD KARTLARI (Tepedeki Özet)
    public class DashboardStatsModel
    {
        public decimal GunlukCiro { get; set; }
        public int GunlukSiparis { get; set; }
        public decimal TahminiStokMaliyeti { get; set; }
        public int KritikStokSayisi { get; set; }
    }

    // 2. EN ÇOK SATANLAR
    public class BestSellerModel
    {
        public string ProductName { get; set; }
        public int ToplamSatisAdeti { get; set; }
        public decimal ToplamCiro { get; set; }
    }

    // 3. PERSONEL PERFORMANS
    public class StaffPerformanceModel
    {
        public string PersonelAdi { get; set; }
        public string Rol { get; set; }
        public int SiparisSayisi { get; set; }
        public int VerimlilikPuani { get; set; }
    }

    // 4. STOK RAPORU
    public class StockReportModel
    {
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public string Durum { get; set; }
        public decimal PotansiyelCiro { get; set; }
    }

    // 5. PASİF KULLANICILAR (Müşteri + Personel)
    public class InactiveUserModel
    {
        public string KullaniciTipi { get; set; }
        public string AdSoyad { get; set; }
        public string Rolu { get; set; }
        public string Iletisim { get; set; }
        public string Durum { get; set; }
        public int GruptakiPasifSayisi { get; set; }
    }

    // 6. DETAYLI İŞLEM RAPORU (View Karşılığı)
   
    public class TransactionModel
    {
        public int SiparisNo { get; set; }
        public DateTime Tarih { get; set; }
        public string SatisNoktasi { get; set; } // Masa 1 veya Paket Servis
        public string Kategori { get; set; }
        public string UrunAdi { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Personel { get; set; }
        public string Durum { get; set; }
    }

    // --- ANA MODEL (Tümünü kapsayan kutu) ---
    public class OwnerDashboardViewModel
    {
        public DashboardStatsModel Stats { get; set; }
        public List<BestSellerModel> BestSellers { get; set; }
        public List<StaffPerformanceModel> StaffPerformance { get; set; }
        public List<StockReportModel> StockReport { get; set; }
        public List<InactiveUserModel> InactiveUsers { get; set; }

        //  İşlem Geçmişi Listesi
        public List<TransactionModel> Transactions { get; set; }
    }
}