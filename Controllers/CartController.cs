using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class CartController : Controller
    {
        private readonly CafeContext _context;

        public CartController(CafeContext context)
        {
            _context = context;
        }

        // 1. BÖLÜM: SEPET İŞLEMLERİ 

        // SEPETİM SAYFASI
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // SEPETE EKLE
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
                var existingItem = cart.FirstOrDefault(x => x.ProductId == id);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.ProductName,
                        Price = product.UnitPrice,
                        Quantity = 1,
                        Image = product.Image
                    });
                }

                HttpContext.Session.SetObject("Cart", cart);
                TempData["Mesaj"] = $"{product.ProductName} sepete eklendi! 🛒";
            }

            return RedirectToAction("Menu", "Customer");
        }

        // SEPETTEN SİL
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");

            if (cart != null)
            {
                var itemToRemove = cart.FirstOrDefault(x => x.ProductId == id);
                if (itemToRemove != null)
                {
                    if (itemToRemove.Quantity > 1)
                    {
                        itemToRemove.Quantity--;
                    }
                    else
                    {
                        cart.Remove(itemToRemove);
                    }

                    HttpContext.Session.SetObject("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }


        // 2. BÖLÜM: SİPARİŞ (CHECKOUT) İŞLEMLERİ 

        // ADRES ONAY EKRANI
        [HttpGet]
        public IActionResult Checkout()
        {
            // 1. Oturumdaki Kullanıcı ID'sini al
            var userId = HttpContext.Session.GetInt32("UserID");

            // Eğer giriş yapılmamışsa Login'e at
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // 2. Sepet boş mu kontrol et
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);

            if (customer == null)
            {
                customer = new Customer
                {
                    UserId = userId.Value,
                    Name = "Yeni",
                    Surname = "Müşteri",
                    Address = "Lütfen adres giriniz..."
                };
            }

            return View(customer);
        }

        [HttpPost]
        public IActionResult CompleteOrder(string Address)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");

            // Güvenlik: Oturum düşmüşse veya sepet boşsa Login'e at
            if (userId == null) return RedirectToAction("Index", "Login");
            if (cart == null || cart.Count == 0) return RedirectToAction("Index");

            var customer = _context.Customers.FirstOrDefault(x => x.UserId == userId);

            if (customer != null)
            {
                // A) MÜŞTERİ ADRESİNİ GÜNCELLE
                customer.Address = Address;
                _context.Update(customer);

                // B) SİPARİŞİ OLUŞTUR
                Order newOrder = new Order();
                newOrder.CustomerId = customer.Id;
                newOrder.OrderDate = DateTime.Now;
                newOrder.Status = true;
                newOrder.ServiceTypeId = 3;

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                // C) DETAYLARI EKLE
                foreach (var item in cart)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.OrderId = newOrder.Id;
                    detail.ProductId = item.ProductId;
                    detail.Quantity = item.Quantity;
                    detail.UnitPrice = item.Price;
                    detail.Status = true;

                    _context.OrderDetails.Add(detail);


                }

                // D) TESLİMAT KAYDI
                DeliveryOrder delivery = new DeliveryOrder();
                delivery.OrderId = newOrder.Id;
                delivery.CustomerId = customer.Id;
                delivery.Status = 1; // Yeni Sipariş

                _context.DeliveryOrders.Add(delivery);

                // Kaydettiğimiz an Trigger 'trg_Stock_DecreaseOnAdd' çalışacak ve stokları düşürecek.
                _context.SaveChanges();

                // E) SEPETİ BOŞALT VE ANA SAYFAYA YÖNLENDİR
                HttpContext.Session.Remove("Cart");

                return RedirectToAction("Index", "Customer", new { siparisDurumu = "basarili", siparisNo = newOrder.Id });
            }

            return RedirectToAction("Index");
        }

        // SİPARİŞ BAŞARILI EKRANI
        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}