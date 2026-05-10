using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class DeliveryController(CafeContext context) : Controller
    {
        private readonly CafeContext _context = context;

        // --- YARDIMCI METOT: LOGLAMA ---
        private void CreateLog(string operationMessage)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId.HasValue)
            {
                var log = new StaffLog
                {
                    StaffId = staffId.Value,
                    Operation = operationMessage,
                    LogDate = DateTime.Now,
                    Status = true
                };
                _context.StaffLogs.Add(log);
            }
        }

        // 1. LİSTELEME SAYFASI (Sadece Aktif Paketler)
        public IActionResult Index()
        {
            // Status 3 (Teslim Edildi) OLMAYANLARI getir.
            var activeDeliveries = _context.DeliveryOrders
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderDetails) // Tutar hesabı için
                .Include(d => d.Order)
                    .ThenInclude(o => o.Customer)     // Adres bilgisi için
                .Where(d => d.Status != 3 && d.Order.Status == true)            // 3: Teslim Edildi demek
                .OrderByDescending(d => d.Id)
                .ToList();

            return View(activeDeliveries);
        }

        // 2. DURUM GÜNCELLE: YOLA ÇIKAR (1 -> 2)
        [HttpPost]
        public IActionResult SetOnTheWay(int id)
        {
            var delivery = _context.DeliveryOrders
                                   .Include(d => d.Order)
                                   .ThenInclude(o => o.Customer)
                                   .FirstOrDefault(x => x.Id == id);

            // Eğer durumu 'Hazırlanıyor' (1) ise 'Yola Çıkar' (2) yap
            if (delivery != null && delivery.Status == 1)
            {
                delivery.Status = 2;

                // Log Tut
                string customerName = delivery.Order?.Customer?.Name ?? "Müşteri";
                CreateLog($"Paket Servis: {customerName} siparişi yola çıktı.");

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 3. TESLİM ET VE KAPAT (2 -> 3)
        // methodId parametresi: 1=Nakit, 2=Kredi Kartı
        [HttpPost]
        public IActionResult SetDelivered(int id, int methodId)
        {
            var delivery = _context.DeliveryOrders
                                   .Include(d => d.Order)
                                   .ThenInclude(o => o.OrderDetails)
                                   .FirstOrDefault(x => x.Id == id);

            // Henüz teslim edilmemişse işlem yap
            if (delivery != null && delivery.Status != 3)
            {
                // A) Durumu Teslim Edildi (3) yap
                delivery.Status = 3;
                // NOT: Bu değişiklik kaydedilince SQL Trigger devreye girip ana siparişi kapatacak.

                // B) Ödeme Yöntemini Güncelle
                delivery.PaymentMethodId = methodId;

                // C) Ödemeyi Al
                var order = delivery.Order;
                if (order != null)
                {
                    // Tutarı Hesapla
                    decimal totalAmount = order.OrderDetails.Sum(x => x.Quantity * x.UnitPrice);

                    // Ödeme Kaydı (Payment)
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = totalAmount,
                        PaymentMethodId = methodId, // Personelin seçtiği yöntem
                        PaymentDate = DateTime.Now,
                        Status = true,
                        UserId = HttpContext.Session.GetInt32("StaffId")
                    };
                    _context.Payments.Add(payment);

                    
                }

                // Log Tut
                string methodText = methodId == 1 ? "Nakit" : "Kredi Kartı";
                CreateLog($"Paket Servis: Sipariş #{delivery.OrderId} teslim edildi. Ödeme: {methodText}.");

                _context.SaveChanges(); // Trigger burada çalışır.
            }

            return RedirectToAction("Index");
        }
    }
}