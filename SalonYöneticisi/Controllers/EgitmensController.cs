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
    public class EgitmensController : Controller
    {
        private readonly SporSalonuContext _context;

        private void SaatleriDoldur()
        {
            var saatler = new List<SelectListItem>();

            // 08-22 
            for (int i = 8; i <= 22; i++)
            {
                saatler.Add(new SelectListItem
                {
                    Text = $"{i}:00", 
                    Value = i.ToString()
                });
            }

            ViewBag.Saatler = new SelectList(saatler, "Value", "Text");
        }

        public EgitmensController(SporSalonuContext context)
        {
            _context = context;
        }

        // GET: Egitmens
        public async Task<IActionResult> Index()
        {
            return View(await _context.Egitmenler.ToListAsync());
        }

        // GET: Egitmens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egitmen = await _context.Egitmenler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egitmen == null)
            {
                return NotFound();
            }

            return View(egitmen);
        }

        // GET: Egitmens/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            SaatleriDoldur();
            return View();
        }

        // POST: Egitmens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,UzmanlikAlani,BaslamaSaati,BitisSaati")] Egitmen egitmen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(egitmen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(egitmen);
        }

        // GET: Egitmens/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egitmen = await _context.Egitmenler.FindAsync(id);
            if (egitmen == null)
            {
                return NotFound();
            }
            SaatleriDoldur();
            return View(egitmen);
        }

        // POST: Egitmens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,UzmanlikAlani,BaslamaSaati,BitisSaati")] Egitmen egitmen)
        {
            if (id != egitmen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(egitmen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EgitmenExists(egitmen.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(egitmen);
        }

        // GET: Egitmens/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egitmen = await _context.Egitmenler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egitmen == null)
            {
                return NotFound();
            }

            return View(egitmen);
        }

        // POST: Egitmens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // egitmenin paketi var mi
            bool paketiVarMi = await _context.HizmetPaketleri.AnyAsync(h => h.EgitmenId == id);

            if (paketiVarMi)
            {
                ViewBag.HataMesaji = "Bu eğitmen şu anda bir veya daha fazla hizmet paketinden sorumlu olduğu için silinemez. Lütfen önce ilgili hizmet paketlerini başka bir eğitmene atayın veya silin.";
                var egitmen = await _context.Egitmenler.FindAsync(id);
                return View(egitmen);
            }

            var egitmenSilinecek = await _context.Egitmenler.FindAsync(id);
            if (egitmenSilinecek != null)
            {
                _context.Egitmenler.Remove(egitmenSilinecek);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EgitmenExists(int id)
        {
            return _context.Egitmenler.Any(e => e.Id == id);
        }
    }
}