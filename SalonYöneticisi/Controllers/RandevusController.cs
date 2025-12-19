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
            // Geçmiş randevuları tamamlandı olarak işaretleme
            var gecmisRandevular = await _context.Randevular
                .Where(r => r.Tarih < DateTime.Today &&
                           (r.Durum == RandevuDurumu.OnayBekliyor || r.Durum == RandevuDurumu.Onaylandi))
                .ToListAsync();

            if (gecmisRandevular.Any())
            {
                foreach (var randevu in gecmisRandevular)
                {
                    randevu.Durum = RandevuDurumu.Tamamlandi;
                }
                // güncellemeleri kaydet
                await _context.SaveChangesAsync();
            }

            var randevular = _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userEmail = User.Identity?.Name;
                randevular = randevular.Where(r => r.Uye.Email == userEmail);
            }

            return View(await randevular.OrderByDescending(r => r.Tarih).ThenBy(r => r.Saat).ToListAsync());
        }

        public async Task<IActionResult> Create(int? paketId, int haftaOffset = 0, string hata = null)
        {
            if (!string.IsNullOrEmpty(hata) && hata == "dolu")
            {
                ModelState.AddModelError("", "Üzgünüz, seçtiğiniz saat sizden hemen önce başkası tarafından alındı. Lütfen sayfayı yenileyip başka bir saat seçin.");
            }

            var paketler = await _context.HizmetPaketleri
                .Include(h => h.Egitmen)
                .ToListAsync();

            var model = new RandevuAlViewModel
            {
                Paketler = paketler,
                SeciliPaketId = paketId
            };

            if (User.IsInRole("Admin"))
            {
                model.Uyeler = await _context.Users.ToListAsync();
            }

            if (paketId.HasValue)
            {
                var seciliPaket = paketler.FirstOrDefault(p => p.Id == paketId);

                if (seciliPaket != null && seciliPaket.Egitmen != null)
                {
                    model.SeciliPaket = seciliPaket;
                    model.Egitmen = seciliPaket.Egitmen;

                    DateTime bugun = DateTime.Today;
                    int bugunIndex = (int)bugun.DayOfWeek;
                    int pazartesiFarki = bugunIndex == 0 ? -6 : 1 - bugunIndex;

                    DateTime buHaftaBasi = bugun.AddDays(pazartesiFarki).AddDays(haftaOffset * 7);
                    model.BaslangicTarihi = buHaftaBasi;

                    var saatDilimleri = new List<TimeSpan>();
                    TimeSpan baslangic = new TimeSpan(seciliPaket.Egitmen.BaslamaSaati, 0, 0);
                    TimeSpan bitis = new TimeSpan(seciliPaket.Egitmen.BitisSaati, 0, 0);

                    while (baslangic < bitis)
                    {
                        saatDilimleri.Add(baslangic);
                        baslangic = baslangic.Add(TimeSpan.FromMinutes(30));
                    }
                    model.SaatDilimleri = saatDilimleri;

                    var randevular = await _context.Randevular
                        .Include(r => r.HizmetPaketi)
                        .Where(r => r.EgitmenId == seciliPaket.EgitmenId
                                 && r.Tarih >= buHaftaBasi
                                 && r.Tarih < buHaftaBasi.AddDays(7)
                                 && r.Durum != RandevuDurumu.Iptal)
                        .ToListAsync();

                    foreach (var r in randevular)
                    {
                        int sure = r.HizmetPaketi?.Sure ?? 30;
                        int slotSayisi = sure / 30;

                        if (TimeSpan.TryParse(r.Saat, out TimeSpan rSaat))
                        {
                            for (int i = 0; i < slotSayisi; i++)
                            {
                                string key = $"{r.Tarih:yyyy-MM-dd}_{rSaat:hh\\:mm}";
                                model.DoluSlotlar.Add(key);
                                rSaat = rSaat.Add(TimeSpan.FromMinutes(30));
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var randevu = await _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var currentUserEmail = User.Identity?.Name;
                if (randevu.Uye != null && randevu.Uye.Email != currentUserEmail)
                {
                    return Unauthorized();
                }
            }

            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            bool adminBaskasinaAliyor = User.IsInRole("Admin") && !string.IsNullOrEmpty(randevu.UyeId);

            if (!adminBaskasinaAliyor)
            {
                var userEmail = User.Identity?.Name;
                var uye = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (uye != null) randevu.UyeId = uye.Id;
            }

            var paket = await _context.HizmetPaketleri.FindAsync(randevu.HizmetPaketiId);
            if (paket != null)
            {
                randevu.EgitmenId = paket.EgitmenId;
            }
            else
            {
                return RedirectToAction("Create");
            }

            bool cakismaVarMi = _context.Randevular.Any(r =>
                r.EgitmenId == randevu.EgitmenId &&
                r.Tarih == randevu.Tarih &&
                r.Saat == randevu.Saat &&
                r.Durum != RandevuDurumu.Iptal);

            if (cakismaVarMi)
            {
                return RedirectToAction("Create", new { paketId = randevu.HizmetPaketiId, hata = "dolu" });
            }

            randevu.Durum = RandevuDurumu.OnayBekliyor;

            ModelState.Remove("UyeId");
            ModelState.Remove("EgitmenId");
            ModelState.Remove("Uye");
            ModelState.Remove("Egitmen");
            ModelState.Remove("HizmetPaketi");

            if (ModelState.IsValid)
            {
                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Create", new { paketId = randevu.HizmetPaketiId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var randevu = await _context.Randevular.Include(r => r.Uye).FirstOrDefaultAsync(r => r.Id == id);
            if (randevu == null) return NotFound();

            if (!User.IsInRole("Admin") && randevu.Uye.Email != User.Identity?.Name)
            {
                return Unauthorized();
            }
            ViewData["EgitmenId"] = new SelectList(_context.Egitmenler, "Id", "AdSoyad", randevu.EgitmenId);
            ViewData["HizmetPaketiId"] = new SelectList(_context.HizmetPaketleri, "Id", "Ad", randevu.HizmetPaketiId);
            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Randevu randevu)
        {
            if (id != randevu.Id) return NotFound();

            ModelState.Remove("Uye");
            ModelState.Remove("Egitmen");
            ModelState.Remove("HizmetPaketi");

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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var randevu = await _context.Randevular
                .Include(r => r.Egitmen)
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Uye)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (randevu == null) return NotFound();

            if (!User.IsInRole("Admin") && randevu.Uye.Email != User.Identity?.Name)
            {
                return Unauthorized();
            }
            return View(randevu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null) _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var randevu = await _context.Randevular.Include(r => r.Uye).FirstOrDefaultAsync(x => x.Id == id);
            if (randevu == null) return NotFound();

            if (!User.IsInRole("Admin") && randevu.Uye.Email != User.Identity?.Name)
            {
                return Unauthorized();
            }

            randevu.Durum = RandevuDurumu.Iptal;
            _context.Update(randevu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}