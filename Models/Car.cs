using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace identityApp.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Marka alanı zorunludur")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model alanı zorunludur")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Yıl alanı zorunludur")]
        [Range(1950, 2100, ErrorMessage = "Geçerli bir yıl giriniz")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Renk alanı zorunludur")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Kilometre alanı zorunludur")]
        [Range(0, int.MaxValue, ErrorMessage = "Kilometre değeri negatif olamaz")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Fiyat alanı zorunludur")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat değeri negatif olamaz")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Motor hacmi alanı zorunludur")]
        public string EngineSize { get; set; }
        
        [Required(ErrorMessage = "Yakıt tipi zorunludur")]
        public string FuelType { get; set; }
        
        [Required(ErrorMessage = "Şanzıman tipi zorunludur")]
        public string Transmission { get; set; }
        
        [Required(ErrorMessage = "Araba tipi zorunludur")]
        public string BodyType { get; set; }
        
        [Range(0, 9, ErrorMessage = "Geçerli bir değer giriniz (0-9)")]
        public int Doors { get; set; }
        
        [Range(0, 10, ErrorMessage = "Geçerli bir değer giriniz (0-10)")]
        public decimal FuelEfficiency { get; set; } // Lt/100km
        
        public string? Features { get; set; } // Opsiyonlar/özellikler JSON olarak saklanabilir
        
        public int? CarTypeId { get; set; }
        
        public CarType? CarType { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        // Silinmiş arabalar için soft delete özelliği
        public bool IsDeleted { get; set; } = false;
        
        // İndirimli fiyat hesaplama
        [NotMapped]
        public decimal? DiscountedPrice 
        { 
            get 
            {
                if (DiscountPercentage.HasValue && DiscountPercentage.Value > 0)
                {
                    return Price * (1 - (DiscountPercentage.Value / 100));
                }
                return null;
            } 
        }
        
        // İndirim yüzdesi (%), null ise indirim yok
        public decimal? DiscountPercentage { get; set; }
        
        // Aracın eklenme tarihi
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        // Kullanıcı favorileri
        public List<FavoriteCar>? FavoritedBy { get; set; }
    }
}