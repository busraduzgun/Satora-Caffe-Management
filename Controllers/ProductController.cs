using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Dropdown için gerekli
using Microsoft.EntityFrameworkCore;      // Include (Tablo birleştirme) için gerekli
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class ProductController : Controller
    {
        private readonly CafeContext _context;

        public ProductController(CafeContext context)
        {
            _context = context;
        }

        // Ürünleri listeleme sayfası
        public IActionResult Index()
        {
            // Sadece Status'u TRUE olanları (Silinmemişleri) getiriyoruz.
            var values = _context.Products
                .Include(x => x.Category)
                .Where(x => x.Status == true) // <-- FİLTRE BURADA
                .ToList();

            return View(values);
        }

        // Yeni ürün ekleme sayfası (Sayfa açılınca çalışır)
        [HttpGet]
        public IActionResult Create()
        {
            List<SelectListItem> categories = (from x in _context.Categories.Where(x => x.Status == true).ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.CategoryName,
                                                   Value = x.Id.ToString()
                                               }).ToList();

            ViewBag.v = categories;
            return View();
        }

        // Yeni ürün kaydetme işlemi 
        [HttpPost]
        public IActionResult Create(Product p)
        {
            p.Status = true; // Yeni ürün her zaman Aktif doğar
            _context.Products.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 4.  PASİFE ALMA İŞLEMİ
        public IActionResult Delete(int id)
        {
            var value = _context.Products.Find(id);

            // Veriyi tamamen silmek yerine durumunu FALSE yapıyoruz.
            if (value != null)
            {
                value.Status = false; // <-- Pasife çek
                _context.Products.Update(value); // <-- Silme değil, Güncelleme yap
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 5. GÜNCELLEME SAYFASINI GETİR (GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            List<SelectListItem> categories = (from x in _context.Categories.Where(x => x.Status == true).ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.CategoryName,
                                                   Value = x.Id.ToString()
                                               }).ToList();
            ViewBag.v = categories;

            var value = _context.Products.Find(id);
            return View(value);
        }

        // 6. GÜNCELLEMEYİ KAYDET (POST)
        [HttpPost]
        public IActionResult Update(Product p)
        {
            // Güncelleme yaparken ürünün hala aktif olduğundan emin oluyoruz.
            p.Status = true;

            _context.Products.Update(p);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}