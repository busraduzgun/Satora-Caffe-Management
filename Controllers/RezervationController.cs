using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class ReservationController : Controller
    {
        private readonly CafeContext _context;

        public ReservationController(CafeContext context)
        {
            _context = context;
        }

        // 1. REZERVASYON FORMUNU GETİR
        [HttpGet]
        public IActionResult Create()
        {
            // Giriş kontrolü
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            // Masaları Listele (Sadece Status=True olan, yani kırık olmayan masalar)
            MasalariDropdownYap();
            
            return View();
        }

        // 2. REZERVASYONU KAYDET (AKILLI KONTROL BURADA)
        [HttpPost]
        public IActionResult Create(Reservation p)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            // Müşteriyi bul
            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);

            // Eğer müşteri bulunamazsa (Güvenlik önlemi)
            if (customer == null) return RedirectToAction("Index", "Customer");

            // --- KONTROL 1: GEÇMİŞ ZAMAN ---
            if (p.ReservationDate < DateTime.Now)
            {
                ViewBag.Hata = "Geçmiş bir tarihe rezervasyon yapamazsınız.";
                MasalariDropdownYap();
                return View(p);
            }
            // --- KONTROL 2: GELİŞMİŞ ÇAKIŞMA KONTROLÜ (1 SAAT KURALI)  ---

            /* Mantık: Müşterinin seçtiği saatin 1 saat öncesi ve 1 saat sonrası 
               arasında başka bir rezervasyon var mı?
               Örnek: Müşteri 14:00 seçti.
               Sistem 13:00 ile 15:00 arasını tarar. Eğer bu arada başka biri varsa izin vermez.
            */

            DateTime baslangicSiniri = p.ReservationDate.AddHours(-1); // Örn: 13:00
            DateTime bitisSiniri = p.ReservationDate.AddHours(1);      // Örn: 15:00

            var cakismaVarMi = _context.Reservations.Any(x =>
                x.TableId == p.TableId &&       // Aynı masa mı?
                x.Status != 2 &&                // İptal edilenler hariç
                x.ReservationDate > baslangicSiniri && // Seçilen zamandan önceki 1 saat içinde mi?
                x.ReservationDate < bitisSiniri        // Seçilen zamandan sonraki 1 saat içinde mi?
            );

            if (cakismaVarMi)
            {
                ViewBag.Hata = "Seçtiğiniz masa, belirttiğiniz saat aralığında doludur. (En az 1 saat ara olmalı).";
                MasalariDropdownYap();
                return View(p);
            }

            // --- HER ŞEY TEMİZSE KAYDET ---
            p.CustomerId = customer.Id;
            p.Status = 0; // Onay Bekliyor olarak başlar
            p.OrderId = null; // Henüz yemek siparişi yok
            
            // Eğer açıklama boşsa varsayılan değer ata
            if (string.IsNullOrEmpty(p.Description)) p.Description = "Web üzerinden rezervasyon";

            _context.Reservations.Add(p);
            _context.SaveChanges();

            TempData["Mesaj"] = "Rezervasyon talebiniz başarıyla oluşturuldu! ✅";
            return RedirectToAction("MyReservations"); 
        }
        // REZERVASYON İPTAL ETME
        public IActionResult CancelReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);

            if (reservation != null)
            {
                // Sadece İptal Edilmemiş (Status != 2) ve Geçmiş Tarihli Olmayanlar İptal Edilebilir
                if (reservation.Status != 2 && reservation.ReservationDate > DateTime.Now)
                {
                    reservation.Status = 2; // 2 = İptal Statüsü
                    _context.SaveChanges();
                    TempData["Mesaj"] = "Rezervasyonunuz başarıyla iptal edildi.";
                }
                else
                {
                    TempData["Hata"] = "Bu rezervasyon artık iptal edilemez.";
                }
            }

            return RedirectToAction("MyReservations");
        }


        private void MasalariDropdownYap()
        {

            var tables = _context.RestaurantTables
                                 .Select(x => new
                                 {
                                     Id = x.Id,
                                     // Masa numarasını ve kapasitesini yazdırılır
                                     Text = "Masa " + x.Id + " (" + x.Capacity + " Kişilik)"
                                 })
                                 .ToList();

            ViewBag.TableList = new SelectList(tables, "Id", "Text");
        }

        // Müşterinin Kendi Rezervasyonlarını Göreceği Sayfa
        public IActionResult MyReservations()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");
            
            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);
            
            var list = _context.Reservations
                               .Include(x => x.Table) // Masa ismini görmek için
                               .Where(x => x.CustomerId == customer.Id)
                               .OrderByDescending(x => x.ReservationDate)
                               .ToList();
                               
            return View(list);
        }
    }
}