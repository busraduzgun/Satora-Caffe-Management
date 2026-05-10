using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Dropdown için
using Microsoft.EntityFrameworkCore;      // Include komutu için
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class StaffController : Controller
    {
        private readonly CafeContext _context;

        public StaffController(CafeContext context)
        {
            _context = context;
        }

        // 1. PERSONEL LİSTELEME
        public IActionResult Index()
        {
            //  Sadece Status'u TRUE olan (Silinmemiş) personeli getir.
            var values = _context.Staff
                .Include(x => x.Role)
                .Where(x => x.Status == true) // <-- FİLTRE BURADA
                .ToList();

            return View(values);
        }

        // 2. PERSONEL EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult Create()
        {
            List<SelectListItem> roleList = (from x in _context.StaffRoles.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.RoleNmae, 
                                                 Value = x.Id.ToString()
                                             }).ToList();

            ViewBag.Roles = roleList;
            return View();
        }

        // 3. PERSONELİ KAYDETME (POST)
        [HttpPost]
        public IActionResult Create(Staff p, string Email, string Password)
        {
            // ADIM 1: Önce yeni bir KULLANICI (User) oluşturuyoruz.
            User yeniKullanici = new User();
            yeniKullanici.Email = Email;
            yeniKullanici.Password = Password;
            yeniKullanici.RoleId = 2; 
            yeniKullanici.Status = true;
            _context.Users.Add(yeniKullanici);
            _context.SaveChanges();

            // ADIM 2: Oluşan Kullanıcı ID'sini, Personel (Staff) nesnesine veriyoruz.
            p.UserId = yeniKullanici.Id;
            p.Status = true; // Personel aktif olsun

            // ADIM 3: Artık Personeli kaydedebiliriz.
            _context.Staff.Add(p);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 4.  PASİFE ALMA
        public IActionResult Delete(int id)
        {
            var value = _context.Staff.Find(id);

            // Personeli veritabanından silmek yerine pasife alıyoruz.
            if (value != null)
            {
                value.Status = false; // Durumu False yap (Görünmez olur)
                _context.Staff.Update(value); 
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 5. GÜNCELLEME EKRANI (GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            List<SelectListItem> roleList = (from x in _context.StaffRoles.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = x.RoleNmae,
                                                 Value = x.Id.ToString()
                                             }).ToList();

            ViewBag.Roles = roleList;

            var value = _context.Staff.Find(id);
            return View(value);
        }

        // 6. GÜNCELLEME KAYDET (POST)
        [HttpPost]
        public IActionResult Update(Staff p)
        {
            // Güncelleme sırasında personelin aktif kaldığından emin oluyoruz.
            p.Status = true;

            _context.Staff.Update(p);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}