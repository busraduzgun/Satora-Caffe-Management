using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    // C# 12 Primary Constructor Yapısı
    public class OperationController(CafeContext context) : Controller
    {
        private readonly CafeContext _context = context;

        // --- YARDIMCI METOT: GÜVENLİ LOG OLUŞTURMA ---
        // Hata alsa bile sistemi durdurmaz.
        private void CreateLog(string operationMessage)
        {
            try
            {
                var staffId = HttpContext.Session.GetInt32("StaffId");

                // KONTROL: ID var mı VE bu ID veritabanında gerçekten mevcut mu?
                if (staffId.HasValue && _context.Staff.Any(s => s.Id == staffId.Value))
                {
                    var log = new StaffLog
                    {
                        StaffId = staffId.Value,
                        Operation = operationMessage,
                        LogDate = DateTime.Now,
                        Status = true
                    };

                    _context.StaffLogs.Add(log);
                    // Not: SaveChanges ana akışta çağrıldığı için burada zorunlu değil
                }
            }
            catch
            {
                // Loglama sırasında hata olursa (örn: foreign key hatası)
                // sistemi durdurma, sessizce devam et.
            }
        }

        // --- GÜNLÜK OPERASYON EKRANI (MASALAR) ---
        public IActionResult Index()
        {
            var tables = _context.RestaurantTables.OrderBy(x => x.Id).ToList();
            return View(tables);
        }

        // --- MASA DETAYI / SİPARİŞ EKRANI ---
        public IActionResult TableDetail(int id)
        {
            var table = _context.RestaurantTables.FirstOrDefault(x => x.Id == id);
            if (table == null) return RedirectToAction("Index");

            var categories = _context.Categories.Where(x => x.Status == true).ToList();
            var products = _context.Products.Where(x => x.Status == true).ToList();

            var activeOrder = _context.Orders
                                      .Include(x => x.OrderDetails)
                                      .ThenInclude(od => od.Product)
                                      .FirstOrDefault(x => x.TableId == id && x.Status == true);

            var model = new OrderViewModel
            {
                Table = table,
                Categories = categories,
                Products = products,
                CurrentOrder = activeOrder
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddToOrder(int tableId, int productId)
        {
            // 1. Masaya ait açık siparişi bul
            var order = _context.Orders
                            .Include(x => x.OrderDetails)
                            .FirstOrDefault(x => x.TableId == tableId && x.Status == true);

            // 2. Eğer açık sipariş yoksa yenisini oluştur
            if (order == null)
            {
                order = new Order
                {
                    TableId = tableId,
                    OrderDate = DateTime.Now,
                    Status = true,
                    ServiceTypeId = 1,
                    StaffId = HttpContext.Session.GetInt32("StaffId")
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
                // SQL Trigger: 'trg_AutoTableStatus' çalışır ve masayı DOLU (Status=1) yapar.

                CreateLog($"Masa {tableId} için yeni sipariş açıldı.");
            }

            // 3. Ürünü Veritabanından Çek
            var product = _context.Products.Find(productId);

            if (product == null) return Content("Ürün Bulunamadı");

            // --- Stok Kontrolü (Sadece UI uyarısı için, düşme işlemini trigger yapacak) ---
            if (product.Stock <= 0)
            {
                return Content("Stok Yetersiz!");
            }

            // 4. Sipariş Detaylarını Yönet
            var detail = order.OrderDetails.FirstOrDefault(x => x.ProductId == productId);

            if (detail != null)
            {
                detail.Quantity++;
                // SQL Trigger: Update olunca 'trg_Stock_UpdateChange' çalışır ve stoğu 1 azaltır.
            }
            else
            {
                detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product.UnitPrice,
                    Status = true
                };
                _context.OrderDetails.Add(detail);
                // SQL Trigger: Insert olunca 'trg_Stock_DecreaseOnAdd' çalışır ve stoğu 1 azaltır.
            }


            CreateLog($"Masa {tableId}: {product.ProductName} eklendi. (Fiyat: {product.UnitPrice})");

            _context.SaveChanges();

            var updatedOrder = _context.Orders
                                    .Include(x => x.OrderDetails)
                                    .ThenInclude(x => x.Product)
                                    .FirstOrDefault(x => x.Id == order.Id);

            return PartialView("_OrderSummary", updatedOrder);
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int detailId)
        {
            var detail = _context.OrderDetails
                                 .Include(x => x.Product)
                                 .FirstOrDefault(x => x.Id == detailId);

            if (detail == null) return Content("Hata");

            if (detail.Product.Stock <= 0)
            {
                return Content("Stok Yetersiz!");
            }

            // Adeti artır (Trigger stoğu düşecek)
            detail.Quantity++;

            CreateLog($"Sipariş Detay {detailId}: {detail.Product.ProductName} adeti artırıldı.");

            _context.SaveChanges();

            var order = _context.Orders
                                .Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Product)
                                .FirstOrDefault(x => x.Id == detail.OrderId);

            return PartialView("_OrderSummary", order);
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int detailId)
        {
            var detail = _context.OrderDetails
                                 .Include(x => x.Product)
                                 .FirstOrDefault(x => x.Id == detailId);

            if (detail == null) return Content("Hata: Ürün bulunamadı");

            string productName = detail.Product?.ProductName ?? "Ürün";
            int currentOrderId = detail.OrderId;

            if (detail.Quantity > 1)
            {
                detail.Quantity--;
                // SQL Trigger: Update olunca aradaki farkı stoğa İADE eder.

                CreateLog($"Sipariş Detay {detailId}: {productName} adeti azaltıldı.");
                _context.SaveChanges();
            }
            else
            {
                // Ürünü siliyoruz
                _context.OrderDetails.Remove(detail);
                // SQL Trigger: Delete olunca stoğu iade eder.

                CreateLog($"Sipariş Detay {detailId}: {productName} iptal edildi/silindi.");

                _context.SaveChanges();

                var order = _context.Orders
                                    .Include(x => x.OrderDetails)
                                    .FirstOrDefault(x => x.Id == currentOrderId);

                // Masa Kapatma Kontrolü (Sipariş tamamen boşaldıysa)
                if (order != null && order.OrderDetails.Count == 0)
                {
                    order.Status = false;

                  

                    CreateLog($"Sipariş {currentOrderId} boş olduğu için otomatik kapatıldı.");

                    _context.SaveChanges(); // SQL Trigger: 'trg_AutoTableStatus' masayı BOŞ yapar.
                    return PartialView("_OrderSummary", null);
                }
            }

            var updatedOrder = _context.Orders
                                    .Include(x => x.OrderDetails)
                                    .ThenInclude(x => x.Product)
                                    .FirstOrDefault(x => x.Id == currentOrderId && x.Status == true);

            return PartialView("_OrderSummary", updatedOrder);
        }

        [HttpPost]
        public IActionResult CloseOrder(int orderId, int paymentMethodId)
        {
            var order = _context.Orders
                                .Include(x => x.OrderDetails)
                                .FirstOrDefault(x => x.Id == orderId);

            if (order == null || order.Status == false) return RedirectToAction("Index");

            decimal totalAmount = 0;
            if (order.OrderDetails.Count > 0)
            {
                totalAmount = order.OrderDetails.Sum(x => x.Quantity * x.UnitPrice);
            }

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = totalAmount,
                PaymentMethodId = paymentMethodId,
                PaymentDate = DateTime.Now,
                Status = true,
                UserId = HttpContext.Session.GetInt32("StaffId")
            };
            _context.Payments.Add(payment);

            order.Status = false;
            order.CloseDate = DateTime.Now;

            // masa durumu güncellemeyi  Trigger yapacak.

            string method = paymentMethodId == 1 ? "Nakit" : "Kredi Kartı";
            CreateLog($"Masa {order.TableId} kapatıldı. Tutar: {totalAmount} TL. Yöntem: {method}.");

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}