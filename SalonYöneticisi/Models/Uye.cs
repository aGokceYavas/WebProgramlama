using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymManagementApp.Models
{
    public class Uye : IdentityUser
    {
        [Display(Name = "Ad Soyad")]
        [StringLength(50)]
        public string? AdSoyad { get; set; } 

        [Display(Name = "Doğum Tarihi")]
        public DateTime? DogumTarihi { get; set; }

        public string? Cinsiyet { get; set; }
    }
}