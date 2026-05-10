using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class LoginController : Controller
    {
        private readonly CafeContext _context;

        public LoginController(CafeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GirisYap(string email, string sifre)
        {
            var kullanici = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == sifre && x.Status == true);

            if (kullanici != null)
            {
                HttpContext.Session.SetInt32("UserID", kullanici.Id);
                HttpContext.Session.SetInt32("RoleID", kullanici.RoleId);
                HttpContext.Session.SetString("Email", kullanici.Email);

                switch (kullanici.RoleId)
                {
                    case 1: return RedirectToAction("Index", "Manager");


                    case 2: // STAFF (GARSON)
                        var staff = _context.Staff
                                            .Include(x => x.Role)
                                            .FirstOrDefault(x => x.UserId == kullanici.Id);

                        if (staff != null)
                        {
                            HttpContext.Session.SetInt32("StaffId", staff.Id);
                            HttpContext.Session.SetString("NameSurname", staff.Name + " " + staff.Surname);
                            HttpContext.Session.SetString("StaffRole", staff.Role.RoleNmae);
                        }

                        
                        return RedirectToAction("Index", "Operation");
                    case 3: return RedirectToAction("Index", "Customer");
                    case 4: return RedirectToAction("Index", "Owner");
                    default: return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                ViewBag.Hata = "Hatalı E-Posta veya Şifre!";
                return View("Index");
            }
        }
        public IActionResult LogOut()
        {
            // 1. Hafızadaki tüm oturum bilgilerini (Giriş yapan personel id, adı vs.) temizle
            HttpContext.Session.Clear();

            

            // 3. Kullanıcıyı Giriş (Login) sayfasına fırlat
            return RedirectToAction("Index", "Login");
        }
    }
}