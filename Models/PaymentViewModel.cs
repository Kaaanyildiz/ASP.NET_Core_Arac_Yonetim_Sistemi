using System.ComponentModel.DataAnnotations;

namespace identityApp.Models
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tutar alanı zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar sıfırdan büyük olmalıdır")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi seçilmelidir")]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
