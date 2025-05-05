using System.ComponentModel.DataAnnotations;

namespace identityApp.Models
{
    public class CarType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tip adı zorunludur")]
        public string Name { get; set; }
        
        public string? Description { get; set; }
        
        // Bu kategorideki araçlar
        public ICollection<Car>? Cars { get; set; }
    }
}