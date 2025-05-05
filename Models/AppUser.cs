using Microsoft.AspNetCore.Identity;

namespace identityApp.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
        // Favori arabalar
        public List<FavoriteCar>? FavoriteCars { get; set; }
    }
}