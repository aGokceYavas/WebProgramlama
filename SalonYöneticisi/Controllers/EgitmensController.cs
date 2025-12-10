using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymManagementApp.Data;
using GymManagementApp.Models;

namespace SalonYöneticisi.Controllers
{
    public class EgitmensController : Controller
    {
        private readonly SporSalonuContext _context;

        public EgitmensController(SporSalonuContext context)
        {
            _context = context;
        }

        // GET: Egitmens
        public async Task<IActionResult> Index()
        {
            var sporSalonuContext = _context.Egitmenler.Include(e => e.Salon);
            return View(await sporSalonuContext.ToListAsync());
        }

        // GET: Egitmens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egitmen = await _context.Egitmenler
                .Include(e => e.Salon)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egitmen == null)
            {
                return NotFound();
            }

            return View(egitmen);
        }

        // GET: Egitmens/Create
        public IActionResult Create()
        {
            ViewData["SalonId"] = new SelectList(_context.Salonlar, "Id", "Ad");
            return View();
        }

        // POST: Egitmens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,UzmanlikAlani,SalonId")] Egitmen egitmen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(egitmen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SalonId"] = new SelectList(_context.Salonlar, "Id", "Ad", egitmen.SalonId);
            return View(egitmen);
        }

        // GET: Egitmens düzenle/5
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
            ViewData["SalonId"] = new SelectList(_context.Salonlar, "Id", "Ad", egitmen.SalonId);
            return View(egitmen);
        }

        // POST: Egitmens düzenle/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,UzmanlikAlani,SalonId")] Egitmen egitmen)
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
            ViewData["SalonId"] = new SelectList(_context.Salonlar, "Id", "Ad", egitmen.SalonId);
            return View(egitmen);
        }

        // GET: Egitmens sil /5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egitmen = await _context.Egitmenler
                .Include(e => e.Salon)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (egitmen == null)
            {
                return NotFound();
            }

            return View(egitmen);
        }

        // POST: Egitmen sil/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var egitmen = await _context.Egitmenler.FindAsync(id);
            if (egitmen != null)
            {
                _context.Egitmenler.Remove(egitmen);
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
