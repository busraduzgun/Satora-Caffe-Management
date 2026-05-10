using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;
using SatoraCaffeRestaurantTracking; // SessionExtensions için

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CafeContext _context;

        public CustomerController(CafeContext context)
        {
            _context = context;
        }

        // 1. ANA SAYFA
        public IActionResult Index()
        {
            var about = _context.Abouts.FirstOrDefault();
            var contact = _context.ContactInfos.FirstOrDefault();
            ViewBag.About = about;
            ViewBag.Contact = contact;
            return View();
        }

        // 2. MENÜ SAYFASI
        public IActionResult Menu()
        {
            var menu = _context.Categories
                               .Include(x => x.Products)
                               .Where(x => x.Status == true) // Sadece aktifler görünüyor
                               .ToList();
            return View(menu);
        }

        // 3. MESAJ GÖNDERME
        [HttpPost]
        public IActionResult SendMessage(Messages p)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId != null)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);
                if (customer != null)
                {
                    p.CustomerId = customer.Id;
                    if (string.IsNullOrEmpty(p.Email))
                    {
                        p.Email = customer.Email;
                    }
                }
            }

            p.SendDate = DateTime.Now;
            p.IsRead = false;

            _context.Messages.Add(p);
            _context.SaveChanges();

            TempData["Mesaj"] = "Mesajınız başarıyla iletildi.";
            return RedirectToAction("Index");
        }

        public IActionResult MyMessages()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);
            if (customer == null) return RedirectToAction("Index", "Login");

            string musteriMaili = customer.Email ?? "";

            var messageList = _context.Messages
                                    .Where(x =>
                                        (musteriMaili != "" && x.Email == musteriMaili)
                                        ||
                                        (x.CustomerId != null && x.CustomerId == customer.Id)
                                    )
                                    .OrderByDescending(x => x.SendDate)
                                    .ToList();

            return View(messageList);
        }

        // 1. PROFİL SAYFASINI GETİR (GET)
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);
            return View(customer);
        }

        // 2. PROFİL GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        public IActionResult ProfileUpdate(Customer p, string newPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);

            if (customer != null)
            {
                customer.Name = p.Name;
                customer.Surname = p.Surname;
                customer.Email = p.Email;
                customer.Telephone = p.Telephone;
                customer.Address = p.Address;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    var user = _context.Users.FirstOrDefault(x => x.Id == customer.UserId);
                    if (user != null)
                    {
                        user.Password = newPassword;
                    }
                }


                _context.SaveChanges();
                TempData["Mesaj"] = "Profiliniz ve şifreniz başarıyla güncellendi.";
            }

            return RedirectToAction("Profile");
        }

        // 4. SİPARİŞLERİM SAYFASI
        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Index", "Login");

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);
            if (customer == null) return View(new List<Order>());

            // Siparişleri çekerken durum ne olursa olsun (İptal dahil) listeliyoruz
            // ki müşteri iptal ettiği siparişi de görebilsin.
            var orders = _context.Orders
                             .Include(x => x.OrderDetails)
                             .Where(x => x.CustomerId == customer.Id)
                             .OrderByDescending(x => x.OrderDate)
                             .ToList();

            return View(orders);
        }

        // ---  SİPARİŞ İPTAL ETME (SOFT DELETE + STOK İADE + LOG) ---
        public IActionResult CancelOrder(int id)
        {
            // 1. İptal edilecek siparişi ve DETAYLARINI (Ürünleri) getir
            // (Find yerine Include kullanıyoruz ki stok iadesi yapabilelim)
            var order = _context.Orders
                                .Include(x => x.OrderDetails)
                                .FirstOrDefault(x => x.Id == id);

            if (order != null)
            {
                // Sadece "Aktif" (Status = true) olan siparişler iptal edilebilir.
                if (order.Status == true)
                {
                  
                    // A) STOK YÖNETİMİ 
                   
                    foreach (var item in order.OrderDetails)
                    {
                        var product = _context.Products.Find(item.ProductId);
                        if (product != null)
                        {
                            // Stoğu iade et
                            product.Stock += item.Quantity;

                            // Eğer ürün "Stok bitti" diye pasif olmuşsa, stok gelince tekrar canlandır
                            if (product.Status == false && product.Stock > 0)
                            {
                                product.Status = true;
                            }
                        }
                    }

                    
                    // B) LOGLAMA 
                    
                    StaffLog log = new StaffLog
                    {
                        StaffId = 24, // Müşteri işlemi olduğu için Sistem/Admin ID'si (1)
                        Operation = $"Sipariş #{id} müşteri panelinden iptal edildi (Soft Delete).",
                        LogDate = DateTime.Now,
                        Status = true
                    };
                    _context.StaffLogs.Add(log);

                   
                    // C) DURUM GÜNCELLEME (SOFT DELETE)
                    // ---------------------------------------------------------
            
                    order.Status = false;

                    // Veritabanında güncelle
                    _context.SaveChanges();

                    TempData["Mesaj"] = "Siparişiniz iptal edildi, ürün stokları iade alındı.";
                }
            }

            return RedirectToAction("MyOrders");
        }
    }
}