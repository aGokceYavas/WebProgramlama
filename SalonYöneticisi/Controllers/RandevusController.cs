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

namespace GymManagementApp.Controllers // Namespace'ini projene göre düzelttim
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

            return View(await randevular.ToListAsync());
        }
        public async Task<IActionResult> Create(int? paketId, int haftaOffset = 0)
        {
            // 1. Paketleri Çek (Dropdown)
            var paketler = await _context.HizmetPaketleri
                .Include(h => h.Egitmen) 
                .ToListAsync();

            // ViewModel
            var model = new RandevuAlViewModel
            {
                Paketler = paketler,
                SeciliPaketId = paketId
            };

            if (User.IsInRole("Admin"))
            {
                model.Uyeler = await _context.Users.ToListAsync();
            }

            // Eğer kullanıcı bir paket seçtiyse hesaplamalara başla
            if (paketId.HasValue)
            {
                var seciliPaket = paketler.FirstOrDefault(p => p.Id == paketId);

                // Paket ve Hocası geçerli mi?
                if (seciliPaket != null && seciliPaket.Egitmen != null)
                {
                    model.SeciliPaket = seciliPaket;
                    model.Egitmen = seciliPaket.Egitmen;

                    // A) Tarih Ayarları
                    DateTime bugun = DateTime.Today;
                    int bugunIndex = (int)bugun.DayOfWeek;
                    // Pazar(0) ise -6 gün git, diğerleri için (1-bugunIndex) kadar git
                    int pazartesiFarki = bugunIndex == 0 ? -6 : 1 - bugunIndex;

                    DateTime buHaftaBasi = bugun.AddDays(pazartesiFarki).AddDays(haftaOffset * 7);
                    model.BaslangicTarihi = buHaftaBasi;

                    // B) Saat Dilimlerini Oluştur (30 dk aralıklarla)
                    var saatDilimleri = new List<TimeSpan>();
                    TimeSpan baslangic = new TimeSpan(seciliPaket.Egitmen.BaslamaSaati, 0, 0);
                    TimeSpan bitis = new TimeSpan(seciliPaket.Egitmen.BitisSaati, 0, 0);

                    // Döngü: Mesai bitene kadar 30 dk ekle
                    while (baslangic < bitis)
                    {
                        saatDilimleri.Add(baslangic);
                        baslangic = baslangic.Add(TimeSpan.FromMinutes(30));
                    }
                    model.SaatDilimleri = saatDilimleri;

                    // C) Dolu Randevuları İşaretle
                    var randevular = await _context.Randevular
                        .Include(r => r.HizmetPaketi)
                        .Where(r => r.EgitmenId == seciliPaket.EgitmenId
                                 && r.Tarih >= buHaftaBasi
                                 && r.Tarih < buHaftaBasi.AddDays(7)
                                 && r.Durum != RandevuDurumu.Iptal)
                        .ToListAsync();

                    foreach (var r in randevular)
                    {
                        // Paket süresine göre kaç slot dolu olacak hesapla
                        int sure = r.HizmetPaketi?.Sure ?? 30;
                        int slotSayisi = sure / 30;

                        TimeSpan rSaat = TimeSpan.Parse(r.Saat.ToString());
                        for (int i = 0; i < slotSayisi; i++)
                        {
                            // "2025-12-16_10:30" formatında anahtar oluştur
                            string key = $"{r.Tarih:yyyy-MM-dd}_{rSaat:hh\\:mm}";
                            model.DoluSlotlar.Add(key);
                            rSaat = rSaat.Add(TimeSpan.FromMinutes(30)); 
                        }
                    }
                }
            }

            return View(model);
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

            if (!User.IsInRole("Admin"))
            {
                var currentUserEmail = User.Identity?.Name;
                // Eğer randevunun sahibi (Uye) yüklendiyse Email kontrolü yap, değilse hata verme
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
                // Admin üye seçmediyse veya normal kullanıcıysa -> Kendisi adına al
                var userEmail = User.Identity?.Name;
                var uye = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (uye != null) randevu.UyeId = uye.Id;
            }

            // 2. Eğitmen Bilgisi 
            var paket = await _context.HizmetPaketleri.FindAsync(randevu.HizmetPaketiId);
            if (paket != null)
            {
                randevu.EgitmenId = paket.EgitmenId;
            }

            // 3. Durum
            randevu.Durum = RandevuDurumu.OnayBekliyor;

            // 4. Zorunlu olmayan validasyonları temizle (Çünkü biz arka planda doldurduk)
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

            // Hata olursa tekrar Create sayfasına (Paket seçili halde) gönder
            return RedirectToAction("Create", new { paketId = randevu.HizmetPaketiId });
        }

        // GET: Randevus/Edit/5
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

        // POST: Randevus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Randevu randevu)
        {
            if (id != randevu.Id) return NotFound();

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
            if (randevu != null) _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.Id == id);
        }
    }
}