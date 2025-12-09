using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementApp.Models
{
    public class Randevu
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tarih")]
        public DateTime Tarih { get; set; }

        [Required]
        [Display(Name = "Saat Aralığı")]
        public string? Saat { get; set; }

        public string? UyeId { get; set; }

        [ForeignKey("UyeId")]
        public virtual Uye? Uye { get; set; }

        [Display(Name = "Eğitmen")]
        public int EgitmenId { get; set; }

        [ForeignKey("EgitmenId")]
        public virtual Egitmen? Egitmen { get; set; }

        [Display(Name = "Hizmet Paketi")]
        public int HizmetPaketiId { get; set; }

        [ForeignKey("HizmetPaketiId")]
        public virtual HizmetPaketi? HizmetPaketi { get; set; }

        public string? Durum { get; set; } = "Bekliyor";
    }
}