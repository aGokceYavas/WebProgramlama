using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonYöneticisi.Data;
using SalonYöneticisi.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GymManagementApp.Data;
using GymManagementApp.Models;

namespace SalonYöneticisi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaporlarApiController : ControllerBase
    {
        private readonly SporSalonuContext _context;

        public RaporlarApiController(SporSalonuContext context)
        {
            _context = context;
        }

        // 1. ENDPOINT: Eğitmen Performans Raporu
        [HttpGet("EgitmenPerformans")]
        public async Task<IActionResult> GetEgitmenPerformans()
        {
            var rapor = await _context.Randevular
                .Include(r => r.HizmetPaketi)
                .Include(r => r.Egitmen)
                .Where(r => r.Durum != RandevuDurumu.Iptal)
                .GroupBy(r => r.Egitmen.AdSoyad)
                .Select(g => new
                {
                    EgitmenAdi = g.Key,
                    RandevuSayisi = g.Count(),
                    ToplamKazanc = g.Sum(r => r.HizmetPaketi.Ucret)
                })
                .OrderByDescending(x => x.RandevuSayisi)
                .ToListAsync();

            return Ok(rapor);
        }
    }
}