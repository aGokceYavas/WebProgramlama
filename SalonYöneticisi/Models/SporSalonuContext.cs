using GymManagementApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymManagementApp.Models;

namespace GymManagementApp.Data
{
    public class SporSalonuContext : IdentityDbContext<Uye>
    {
        public SporSalonuContext(DbContextOptions<SporSalonuContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // HizmetPaketi ile Egitmen arasındaki ilişkiyi yapılandır
            modelBuilder.Entity<HizmetPaketi>()
                .HasOne(h => h.Egitmen)
                .WithMany() 
                .HasForeignKey(h => h.EgitmenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Egitmen> Egitmenler { get; set; }
        public DbSet<HizmetPaketi> HizmetPaketleri { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
    }
}