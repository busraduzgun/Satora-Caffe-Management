using Microsoft.AspNetCore.Mvc;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class CategoryController : Controller
    {
        // Veritabanı bağlantı nesnesi
        private readonly CafeContext _context;

        // Constructor (Yapıcı Metot) - Veritabanı bağlantısını başlatır
        public CategoryController(CafeContext context)
        {
            _context = context;
        }

        // 1. Kategori Listeleme İşlemi
        public IActionResult Index()
        {
            //  Sadece Status'u TRUE olan (Silinmemiş) kategorileri getir.
            var values = _context.Categories.Where(x => x.Status == true).ToList();
            return View(values);
        }

        // 2. Yeni Kategori Ekleme Sayfası (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. Veritabanına Kaydetme İşlemi (POST)
        [HttpPost]
        public IActionResult Create(Category p)
        {
            p.Status = true; // Yeni kategori her zaman Aktif doğar
            _context.Categories.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 4.  PASİFE ALMA
        public IActionResult Delete(int id)
        {
            // 1. Silinecek veriyi ID'sine göre bul
            var value = _context.Categories.Find(id);

            //  Veriyi tamamen silmek yerine durumunu FALSE yapıyoruz.
            if (value != null)
            {
                value.Status = false; // Pasife çek
                _context.Categories.Update(value); // Remove yerine Update kullan
                _context.SaveChanges(); // Kaydet
            }

            // Listeye geri dön
            return RedirectToAction("Index");
        }

        // 5. GÜNCELLEME SAYFASINI GETİR (GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            // Güncellenecek kategoriyi ID'sine göre bul
            var value = _context.Categories.Find(id);

            // Bulunan veriyi kutucukların içine dolsun diye sayfaya gönder
            return View(value);
        }

        // 6. GÜNCELLEMEYİ KAYDET (POST)
        [HttpPost]
        public IActionResult Update(Category p)
        {
            //  Güncellerken yanlışlıkla pasife düşmemesi için true yapıyoruz.
            p.Status = true;

            // Değişiklikleri veritabanına uygula
            _context.Categories.Update(p);

            // Kaydet
            _context.SaveChanges();

            // Listeye dön
            return RedirectToAction("Index");
        }
    }
}