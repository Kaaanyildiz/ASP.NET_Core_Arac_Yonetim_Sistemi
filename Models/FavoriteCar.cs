namespace identityApp.Models
{
    public class FavoriteCar
    {
        public int Id { get; set; }
        
        // Favoriye ekleyen kullanıcı
        public string UserId { get; set; }
        public AppUser User { get; set; }
        
        // Favoriye eklenen araba
        public int CarId { get; set; }
        public Car Car { get; set; }
        
        // Favoriye eklenme tarihi
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}