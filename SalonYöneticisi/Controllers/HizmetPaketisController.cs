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

namespace SalonYöneticisi.Controllers
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
            return View(await _context.HizmetPaketleri.ToListAsync());
        }

        // GET: HizmetPaketis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmetPaketi = await _context.HizmetPaketleri
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hizmetPaketi == null)
            {
                return NotFound();
            }

            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: HizmetPaketis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,Sure,Ucret")] HizmetPaketi hizmetPaketi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hizmetPaketi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmetPaketi = await _context.HizmetPaketleri.FindAsync(id);
            if (hizmetPaketi == null)
            {
                return NotFound();
            }
            return View(hizmetPaketi);
        }

        // POST: HizmetPaketis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Sure,Ucret")] HizmetPaketi hizmetPaketi)
        {
            if (id != hizmetPaketi.Id)
            {
                return NotFound();
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
                    if (!HizmetPaketiExists(hizmetPaketi.Id))
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
            return View(hizmetPaketi);
        }

        // GET: HizmetPaketis/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmetPaketi = await _context.HizmetPaketleri
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hizmetPaketi == null)
            {
                return NotFound();
            }

            return View(hizmetPaketi);
        }

        // POST: HizmetPaketis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}
