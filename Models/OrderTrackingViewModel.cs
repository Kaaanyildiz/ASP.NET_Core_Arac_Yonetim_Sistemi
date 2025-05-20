using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace identityApp.Models
{
    public class OrderTrackingViewModel
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string CustomerEmail { get; set; } = string.Empty;

        public List<Order>? Orders { get; set; }
    }
}
