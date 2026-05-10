using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SatoraCaffeRestaurantTracking.Models;
using System.Linq;

namespace SatoraCaffeRestaurantTracking.Controllers
{
    public class WebsiteController : Controller
    {
        private readonly CafeContext _context;

        public WebsiteController(CafeContext context)
        {
            _context = context;
        }

        // 1. MESAJLAR
        public IActionResult Messages()
        {
            var values = _context.Messages
                                 .Include(x => x.Customer)
                                 .OrderByDescending(x => x.SendDate)
                                 .ToList();
            return View(values);
        }
        [HttpPost]
        public IActionResult SaveReply(int id, string replyText)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                message.ReplyContent = replyText; // Yazdığın cevabı kaydet
                message.ReplyDate = DateTime.Now; // Şu anki saati kaydet
                _context.SaveChanges(); // Veritabanını güncelle
            }
            return RedirectToAction("Messages");
        }

        // 2. HAKKIMIZDA
        [HttpGet]
        public IActionResult AboutSettings()
        {
            var value = _context.Abouts.FirstOrDefault();
            return View(value);
        }

        [HttpPost]
        public IActionResult AboutSettings(Abouts a) 
        {
            _context.Abouts.Update(a);
            _context.SaveChanges();
            return RedirectToAction("AboutSettings");
        }

        // 3. İLETİŞİM
        [HttpGet]
        public IActionResult ContactSettings()
        {
            var value = _context.ContactInfos.FirstOrDefault();
            return View(value);
        }

        [HttpPost]
        public IActionResult ContactSettings(ContactInfos c) 
        {
            _context.ContactInfos.Update(c);
            _context.SaveChanges();
            return RedirectToAction("ContactSettings");
        }

        // 4. MESAJ SİL
        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            if (value != null)
            {
                _context.Messages.Remove(value);
                _context.SaveChanges();
            }
            return RedirectToAction("Messages");
        }
    }
}