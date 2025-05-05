using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Models
{
    public class IdentityContext : IdentityDbContext<AppUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options) { }
            
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<FavoriteCar> FavoriteCars { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Seed data IdentitySeedData sınıfına taşındı, burada tekrar etmiyoruz
            
            // FavoriteCar için ilişki yapılandırması
            builder.Entity<FavoriteCar>()
                .HasOne(fc => fc.User)
                .WithMany(u => u.FavoriteCars)
                .HasForeignKey(fc => fc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<FavoriteCar>()
                .HasOne(fc => fc.Car)
                .WithMany(c => c.FavoritedBy)
                .HasForeignKey(fc => fc.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}