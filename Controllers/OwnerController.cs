using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class OwnerController(CafeContext context) : Controller
    {
        private readonly CafeContext _context = context;

        public IActionResult Index()
        {
           

            var model = new OwnerDashboardViewModel();

            try
            {
                // 1. DASHBOARD İSTATİSTİKLERİ (Tek Satır Döner)
                // "FromSqlRaw" metodu SQL'deki SP'yi çalıştırır.
                model.Stats = _context.DashboardStatsReports
                    .FromSqlRaw("EXEC sp_GetDashboardStats")
                    .AsEnumerable()
                    .FirstOrDefault() ?? new DashboardStatsModel();

                // 2. EN ÇOK SATANLAR LİSTESİ
                model.BestSellers = _context.BestSellerReports
                    .FromSqlRaw("EXEC sp_GetBestSellers")
                    .ToList();

                // 3. PERSONEL PERFORMANS LİSTESİ
                model.StaffPerformance = _context.StaffPerformanceReports
                    .FromSqlRaw("EXEC sp_GetStaffPerformance")
                    .ToList();

                // 4. STOK RAPORU
                model.StockReport = _context.StockReports
                    .FromSqlRaw("EXEC sp_GetStockReport")
                    .ToList();

                // 5. PASİF KULLANICILAR (Müşteri + Personel)
                model.InactiveUsers = _context.InactiveUserReports
                    .FromSqlRaw("EXEC sp_GetInactiveUsersReport")
                    .ToList();

                // 6. DETAYLI İŞLEM GEÇMİŞİ (View'den çekiyoruz - )
                // Veritabanındaki 'vw_OrderDetailsFull' view'ini kullanır.
                // Performans için sadece son 50 işlemi getiriyoruz.
                model.Transactions = _context.TransactionReports
                    .OrderByDescending(x => x.SiparisNo)
                    .Take(50)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Hata alırsak boş model dönelim, sayfa patlamasın
                ViewBag.Error = "Veriler çekilirken hata oluştu: " + ex.Message;

                model.Stats = new DashboardStatsModel();
                model.BestSellers = new List<BestSellerModel>();
                model.StaffPerformance = new List<StaffPerformanceModel>();
                model.StockReport = new List<StockReportModel>();
                model.InactiveUsers = new List<InactiveUserModel>();
                model.Transactions = new List<TransactionModel>(); // Hata durumunda boş liste
            }

            return View(model);
        }
    }
}