using System.ComponentModel.DataAnnotations;

namespace GymManagementApp.Models
{
    public class Salon
    {
        public int Id { get; set; }

        [Display(Name = "Salon Adı")]
        [Required(ErrorMessage = "Salon adı boş bırakılamaz")]
        public string? Ad { get; set; } 

        public string? Adres { get; set; }

        public string? Telefon { get; set; }

        public virtual ICollection<Egitmen> Egitmenler { get; set; } = new List<Egitmen>();
    }
}