using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementApp.Models
{
    public class Egitmen
    {
        public int Id { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Eğitmen adı zorunludur")]
        public string? AdSoyad { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string? UzmanlikAlani { get; set; }

        [Display(Name = "Çalıştığı Salon")]
        public int SalonId { get; set; }

        [ForeignKey("SalonId")]
        public virtual Salon? Salon { get; set; }

        public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}