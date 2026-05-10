using Microsoft.AspNetCore.Mvc;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class RegisterController : Controller
    {
        private readonly CafeContext _context;

        public RegisterController(CafeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Customer p, string Password, string Email)
        {
            // 1. KONTROL: Bu mail adresi zaten var mı?
            var emailCheck = _context.Users.FirstOrDefault(x => x.Email == Email);
            if (emailCheck != null)
            {
                ViewBag.Hata = "Bu mail adresi zaten sistemde kayıtlı!";
                return View();
            }

          
            // 2. ADIM: USERS TABLOSUNA KAYIT (Giriş Bilgileri)
            
            User newUser = new User()
            {
                Email = Email,
                Password = Password,
                Status = true,  // Aktif kullanıcı
                RoleId = 3      // 3 = CUSTOMER (Senin LoginController switch yapısına göre)
            };

            _context.Users.Add(newUser);
            _context.SaveChanges(); // Kaydediyoruz ki ID oluşsun!

           
            // 3. ADIM: CUSTOMERS TABLOSUNA KAYIT (Profil Bilgileri)
         
            p.UserId = newUser.Id; // Yukarıda oluşan User ID'yi buraya bağlıyoruz!
            p.Status = true;       // Müşteri profili de aktif
            p.Email = Email;

            _context.Customers.Add(p);
            _context.SaveChanges();


            // 4. ADIM: OTOMATİK GİRİŞ (AUTO LOGIN) 


            // Kullanıcı ID
            HttpContext.Session.SetInt32("UserID", newUser.Id);

            // Rol ID (Müşteri olduğu için 3)
            HttpContext.Session.SetInt32("RoleID", 3);

            // Email
            HttpContext.Session.SetString("Email", newUser.Email);

            // Direkt Müşteri Paneline Fırlat
            return RedirectToAction("Index", "Customer");
        }
    }
}