using GymManagementApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApp.Data
{
    public class SporSalonuContext : IdentityDbContext<Uye>
    {
        public SporSalonuContext(DbContextOptions<SporSalonuContext> options)
            : base(options)
        {
        }

        public DbSet<Salon> Salonlar { get; set; }
        public DbSet<Egitmen> Egitmenler { get; set; }
        public DbSet<HizmetPaketi> HizmetPaketleri { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
    }
}