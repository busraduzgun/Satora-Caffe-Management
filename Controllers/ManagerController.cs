using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Session (Oturum) kontrolü için

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            // KONTROL: İçeri girmeye çalışan kişi giriş yapmış mı? Ve Yönetici mi?
            // (RoleID: 1 -> Yönetici demektir)
            if (HttpContext.Session.GetInt32("UserID") == null || HttpContext.Session.GetInt32("RoleID") != 1)
            {
                // Değilse, giriş sayfasına geri postala
                return RedirectToAction("Index", "Login");
            }

            // Her şey yolundaysa Müdür Ekranını aç
            return View();
        }
    }
}