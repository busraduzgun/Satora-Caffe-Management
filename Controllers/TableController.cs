using Microsoft.AspNetCore.Mvc;
using SatoraCaffeRestaurantTracking.Models;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class TableController : Controller
    {
        private readonly CafeContext _context;

        public TableController(CafeContext context)
        {
            _context = context;
        }

        // 1. MASALARI LİSTELEME
        public IActionResult Index()
        {
            // Veritabanındaki tüm masaları getirir
            var values = _context.RestaurantTables.ToList();
            return View(values);
        }

        // 2. YENİ MASA EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Sayfa açıldığında sadece boş form gösterir
            return View();
        }

        // 3. YENİ MASAYI KAYDETME (POST)
        [HttpPost]
        public IActionResult Create(RestaurantTable t)
        {

            // 0 = Boş, 1 = Dolu olarak kabul ediyoruz.
            // Yeni masa eklerken varsayılan olarak 0 (Boş) yapıyoruz.
            t.Status = 0;

            _context.RestaurantTables.Add(t);
            _context.SaveChanges();

            // İşlem bitince listeye geri dön
            return RedirectToAction("Index");
        }

        // 4. MASA SİLME
        public IActionResult Delete(int id)
        {
            // 1. KONTROL: Bu masada şu an açık (kapanmamış) bir sipariş var mı?
            // Orders tablosunda TableId'si bu olan VE Status'u true (Aktif) olan kayıt var mı?
            bool masaDoluMu = _context.Orders.Any(x => x.TableId == id && x.Status == true);

            if (masaDoluMu)
            {
                // Eğer masa doluysa silme işlemini yapma!
                // Kullanıcıya hata mesajı göndermek için TempData kullanıyoruz.
                TempData["Hata"] = "Bu masa şu an dolu! Silmek için önce hesabı kapatmalısınız.";
                return RedirectToAction("Index");
            }

            // 2. KONTROL: Masa boşsa "Soft Delete" (Pasife Alma) işlemini yap
            var value = _context.RestaurantTables.Find(id);
            if (value != null)
            {
                value.Status = 0; // Masayı pasife al (Silinmiş gibi göster)
                _context.RestaurantTables.Update(value);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        // 5. GÜNCELLEME SAYFASINI GETİR (GET)
        [HttpGet]
        public IActionResult Update(int id)
        {
            var value = _context.RestaurantTables.Find(id);
            return View(value);
        }

        // 6. GÜNCELLEMEYİ KAYDET (POST)
        [HttpPost]
        public IActionResult Update(RestaurantTable t)
        {
            _context.RestaurantTables.Update(t);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}