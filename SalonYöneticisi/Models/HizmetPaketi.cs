using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementApp.Models
{
    public class HizmetPaketi
    {
        public int Id { get; set; }

        [Display(Name = "Paket Adı")]
        [Required(ErrorMessage = "Paket adı zorunludur")]
        public string? Ad { get; set; }

        [Display(Name = "Süre (Dakika)")]
        public int Sure { get; set; }

        [Display(Name = "Ücret")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Ucret { get; set; }

        [Display(Name = "Paket İçeriği")]
        [DataType(DataType.MultilineText)]
        public string? Aciklama { get; set; }

        [Display(Name = "Eğitmen")]
        public int EgitmenId { get; set; }

        // Navigation Property (Veritabanı ilişkisi için)
        public Egitmen? Egitmen { get; set; }
    }
}