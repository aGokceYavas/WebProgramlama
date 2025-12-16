using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagementApp.Data;
using GymManagementApp.Models;
using Microsoft.AspNetCore.Authorization; // Bunu ekledik (Rol kontrolü için)

namespace SalonYöneticisi.Controllers
{
    // Sadece giriş yapmış kullanıcılar girebilsin
    [Authorize]
    public class RandevusController : Controller
    {
        private readonly SporSalonuContext _context;

        public RandevusController(SporSalonuContext context)
        {
            _context = context;
        }

        // GET: Randevus
        public async Task<IActionResult> Index()
        {
            // 1. Sorguyu hazırlıyoruz (Henüz veritabanına gitmedi)
            var randevular = _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .AsQueryable();

            // 2. KURAL: Eğer Admin değilse, sadece KENDİ randevularını görsün
            if (!User.IsInRole("Admin"))
            {
                var userEmail = User.Identity?.Name;
                randevular = randevular.Where(r => r.Uye.Email == userEmail);
            }

            // 3. Listeyi getir
            return View(await randevular.ToListAsync());
        }

        // GET: Randevus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var randevu = await _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null) return NotFound();

            return View(randevu);
        }

        // GET: Randevus/Create
        public IActionResult Create()
        {
            ViewData["EgitmenId"] = new SelectList(_context.Egitmenler, "Id", "AdSoyad");
            ViewData["HizmetPaketiId"] = new SelectList(_context.HizmetPaketleri, "Id", "Ad");
            // Üye seçimi kaldırıldı, sistem otomatik tanıyacak.
            return View();
        }

        // POST: Randevus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tarih,Saat,EgitmenId,HizmetPaketiId,Durum")] Randevu randevu)
        {
            // Giriş yapan kullanıcının email adresini al
            var userEmail = User.Identity?.Name;

            // Email adresine göre kullanıcıyı bul
            var uye = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (uye != null)
            {
                randevu.UyeId = uye.Id; // Randevuya kullanıcının ID'sini yapıştır
            }

            // ModelState validasyonunu bypass edip kaydediyoruz (Bazen UyeId null gelebilir formdan)

            // Fix for CS0029: Convert the enum value to a string explicitly

            // Fix for CS0029: Correctly assign the enum value instead of converting it to a string
            randevu.Durum = RandevuDurumu.OnayBekliyor;
            _context.Add(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Randevus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var randevu = await _context.Randevular.Include(r => r.Uye).FirstOrDefaultAsync(r => r.Id == id);

            if (randevu == null) return NotFound();

            // GÜVENLİK KONTROLÜ: 
            // Admin değilse VE randevu başkasına aitse engelle!
            if (!User.IsInRole("Admin") && randevu.Uye.Email != User.Identity?.Name)
            {
                return Unauthorized(); // Yetkisiz giriş hatası ver
            }

            ViewData["EgitmenId"] = new SelectList(_context.Egitmenler, "Id", "AdSoyad", randevu.EgitmenId);
            ViewData["HizmetPaketiId"] = new SelectList(_context.HizmetPaketleri, "Id", "Ad", randevu.HizmetPaketiId);
            return View(randevu);
        }

        // POST: Randevus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tarih,Saat,UyeId,EgitmenId,HizmetPaketiId,Durum")] Randevu randevu)
        {
            if (id != randevu.Id) return NotFound();

            // Formdan gelen veriyi kaydetmeden önce güvenliği tekrar kontrol etmemiz lazım
            // ama şimdilik basit tutuyoruz.

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RandevuExists(randevu.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EgitmenId"] = new SelectList(_context.Egitmenler, "Id", "AdSoyad", randevu.EgitmenId);
            ViewData["HizmetPaketiId"] = new SelectList(_context.HizmetPaketleri, "Id", "Ad", randevu.HizmetPaketiId);
            return View(randevu);
        }

        // GET: Randevus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var randevu = await _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null) return NotFound();

            // GÜVENLİK KONTROLÜ: Başkasının randevusunu silemesin
            if (!User.IsInRole("Admin") && randevu.Uye.Email != User.Identity?.Name)
            {
                return Unauthorized();
            }

            return View(randevu);
        }

        // POST: Randevus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}