using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class DashboardController : Controller
    {
        private readonly CafeContext _context;

        public DashboardController(CafeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            var bugun = DateTime.Today; // Bugün 00:00

            // 1. GÜNLÜK OPERASYON ÖZETİ 
            //SP KULLANIYORUZ

            // SQL'den veriyi çekiyoruz (Ciro ve Sipariş Sayısı buradan geliyor)
            var stats = _context.DashboardRawStats
                                .FromSqlRaw("EXEC sp_GetDashboardStats")
                                .AsEnumerable()
                                .FirstOrDefault();

            if (stats != null)
            {
                // SP'den gelen verileri modele aktarıyoruz
                model.GunlukCiro = stats.GunlukCiro;
                model.MusteriSayisi = stats.GunlukSiparis;
            }
            else
            {
                model.GunlukCiro = 0;
                model.MusteriSayisi = 0;
            }

            // Açık Sipariş (Masalar)
            model.AcikSiparis = _context.RestaurantTables.Count(x => x.Status == 1);


           
            // 2. KRİTİK STOK TAKİBİ 
            
            model.KritikStoklar = _context.Products
                                          .Where(x => x.Stock < 20 && x.Status == true)
                                          .OrderBy(x => x.Stock)
                                          .Take(5)
                                          .ToList();


            
            // 3. ANLIK YOĞUNLUK ANALİZİ
            
            var bugunSiparisleri = _context.Orders
                                           .Where(x => x.OrderDate.Date == bugun)
                                           .Select(x => x.OrderDate.Hour)
                                           .ToList();

            model.SaatlikYogunluk = new Dictionary<string, int>();

            if (bugunSiparisleri.Any())
            {
                var gruplanmis = bugunSiparisleri.GroupBy(x => x)
                                                 .Select(g => new { Saat = g.Key, Adet = g.Count() })
                                                 .OrderBy(x => x.Saat);

                foreach (var item in gruplanmis)
                {
                    model.SaatlikYogunluk.Add($"{item.Saat}:00", item.Adet);
                }
            }
            else
            {
                model.SaatlikYogunluk.Add("Henüz Veri Yok", 0);
            }


         
            // 4. EN ÇOK SATILANLAR 
            //SP KULLANIYORUZ
         

            // SQL'den en çok satanları çekiyoruz
            var bestSellers = _context.BestSellerRawStats
                                      .FromSqlRaw("EXEC sp_GetBestSellers")
                                      .ToList();

            model.EnCokSatilanlar = new Dictionary<string, int>();

            if (bestSellers.Any())
            {
                foreach (var item in bestSellers)
                {
                    // SP'den gelen sonucu  Dictionary yapısına uyduruyoruz
                    model.EnCokSatilanlar.Add(item.ProductName, item.ToplamSatisAdeti);
                }
            }
            else
            {
                model.EnCokSatilanlar.Add("Satış Yok", 1);
            }


           
            // 5. SİPARİŞ PROBLEMLERİ
       
            var kritikSure = DateTime.Now.AddMinutes(-45);

            model.GecikenSiparis = _context.Orders
                                           .Count(x => x.Status == true && x.OrderDate < kritikSure);

            model.IptalSiparis = 0;

            return View(model);
        }
    }
}