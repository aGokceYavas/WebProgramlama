using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagementApp.Data;
using GymManagementApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymManagementApp.Controllers
{

    public class HizmetPaketisController : Controller
    {
        private readonly SporSalonuContext _context;

        public HizmetPaketisController(SporSalonuContext context)
        {
            _context = context;
        }

        // GET: HizmetPaketis
        public async Task<IActionResult> Index()
        {
            // Eğitmen bilgisini de çekiyoruz ki listede adı görünsün
            var sporSalonuContext = _context.HizmetPaketleri.Include(h => h.Egitmen);
            return View(await sporSalonuContext.ToListAsync());
        }

        // GET: HizmetPaketis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hizmetPaketi = await _context.HizmetPaketleri
                .Include(h => h.Egitmen) // Detayda da hoca görünsün
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmetPaketi == null) return NotFound();

            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            EgitmenListesiHazirla();
            return View();
        }

        // POST: HizmetPaketis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // Bind içine 'EgitmenId' ve 'Aciklama' eklendi
        public async Task<IActionResult> Create([Bind("Id,Ad,Sure,Ucret,Aciklama,EgitmenId")] HizmetPaketi hizmetPaketi)
        {
            // EK GÜVENLİK: Süre 30, 60, 90, 120 olabilir
            var gecerliSureler = new[] { 30, 60, 90, 120 };
            if (!gecerliSureler.Contains(hizmetPaketi.Sure))
            {
                ModelState.AddModelError("Sure", "Hizmet süresi 30, 60, 90 veya 120 dakika olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(hizmetPaketi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa listeyi tekrar doldur
            EgitmenListesiHazirla(hizmetPaketi.EgitmenId);
            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hizmetPaketi = await _context.HizmetPaketleri.FindAsync(id);
            if (hizmetPaketi == null) return NotFound();

            EgitmenListesiHazirla(hizmetPaketi.EgitmenId);
            return View(hizmetPaketi);
        }

        // POST: HizmetPaketis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Sure,Ucret,Aciklama,EgitmenId")] HizmetPaketi hizmetPaketi)
        {
            if (id != hizmetPaketi.Id) return NotFound();

            // EK GÜVENLİK: Süre kontrolü
            var gecerliSureler = new[] { 30, 60, 90, 120 };
            if (!gecerliSureler.Contains(hizmetPaketi.Sure))
            {
                ModelState.AddModelError("Sure", "Hizmet süresi 30, 60, 90 veya 120 dakika olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hizmetPaketi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HizmetPaketiExists(hizmetPaketi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            EgitmenListesiHazirla(hizmetPaketi.EgitmenId);
            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hizmetPaketi = await _context.HizmetPaketleri
                .Include(h => h.Egitmen)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmetPaketi == null) return NotFound();

            return View(hizmetPaketi);
        }

        // POST: HizmetPaketis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmetPaketi = await _context.HizmetPaketleri.FindAsync(id);
            if (hizmetPaketi != null)
            {
                _context.HizmetPaketleri.Remove(hizmetPaketi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HizmetPaketiExists(int id)
        {
            return _context.HizmetPaketleri.Any(e => e.Id == id);
        }

        // Yardımcı Metot: Dropdown Listesini Hazırlar
        private void EgitmenListesiHazirla(int? seciliId = null)
        {
            var egitmenQuery = _context.Egitmenler.Select(e => new
            {
                Id = e.Id,
                // Görünen İsim: Ahmet Yılmaz (Pilates)
                Gorunum = e.AdSoyad + " (" + e.UzmanlikAlani + ")"
            });

            ViewData["EgitmenId"] = new SelectList(egitmenQuery, "Id", "Gorunum", seciliId);
        }
    }
}