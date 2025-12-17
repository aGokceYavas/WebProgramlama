using System;
using System.Collections.Generic;

namespace GymManagementApp.Models
{
    public class RandevuAlViewModel
    {
        // 1. Paket Seçimi İçin
        public int? SeciliPaketId { get; set; }
        public List<HizmetPaketi> Paketler { get; set; } = new List<HizmetPaketi>();

        // 2. Seçilen Paketin Detayları (Ekranda göstermek için)
        public HizmetPaketi? SeciliPaket { get; set; }
        public Egitmen? Egitmen { get; set; }

        // 3. Takvim Ayarları
        public DateTime BaslangicTarihi { get; set; } // Pazartesi
        public List<TimeSpan> SaatDilimleri { get; set; } = new List<TimeSpan>(); // 09:00, 09:30...

        // 4. Dolu Olan Slotlar (Hangi saatler meşgul?)
        // Format: "2025-12-16_09:30"
        public HashSet<string> DoluSlotlar { get; set; } = new HashSet<string>();

        public List<Uye> Uyeler { get; set; } = new List<Uye>();
    }
}