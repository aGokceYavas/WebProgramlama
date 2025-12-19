using GymManagementApp.Models;
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
        public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();

        [Display(Name = "Mesai Başlangıç Saati")]
        [Range(0, 23)]
        public int BaslamaSaati { get; set; } = 9; 

        [Display(Name = "Mesai Bitiş Saati")]
        [Range(0, 23)]
        public int BitisSaati { get; set; } = 18; 
    }
}