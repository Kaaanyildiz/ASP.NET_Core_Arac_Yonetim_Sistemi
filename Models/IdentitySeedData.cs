using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Admin1234";
        private const string adminRole = "Admin";


        public static async Task IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateAsyncScope().ServiceProvider.GetRequiredService<IdentityContext>();
            
            // Veritabanını oluştur veya güncelle
            context.Database.Migrate();
            
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Admin rolünü oluştur
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var user = await userManager.FindByNameAsync(adminUser);
            
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = adminUser,
                    Email = "admin@fatmanurkilickaya.com",
                    PhoneNumber = "33333",
                    EmailConfirmed = true,
                    FullName = "Admin User"
                };

                await userManager.CreateAsync(user, adminPassword);
                await userManager.AddToRoleAsync(user, adminRole);
            }
            else 
            {
                // Kullanıcı zaten varsa, admin rolünde olduğundan emin ol
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }

            // Araç kategorilerini ekle
            if (!context.CarTypes.Any())
            {
                context.CarTypes.AddRange(
                    new CarType { Name = "Sedan", Description = "Klasik 4 kapılı aile arabası" },
                    new CarType { Name = "SUV", Description = "Spor aktivite aracı" },
                    new CarType { Name = "Hatchback", Description = "Kompakt kasa tipi" },
                    new CarType { Name = "Coupe", Description = "Sportif, 2 kapılı otomobil" },
                    new CarType { Name = "Station Wagon", Description = "Geniş bagaj hacimli aile arabası" },
                    new CarType { Name = "Convertible", Description = "Açılabilir tavanlı otomobil" },
                    new CarType { Name = "MPV", Description = "Çok amaçlı araç" }
                );
                context.SaveChanges();
            }

            // CarTypes verilerini çekelim (ID'leri kullanmak için)
            var sedanType = context.CarTypes.FirstOrDefault(t => t.Name == "Sedan");
            var suvType = context.CarTypes.FirstOrDefault(t => t.Name == "SUV");
            var hatchbackType = context.CarTypes.FirstOrDefault(t => t.Name == "Hatchback");
            var coupeType = context.CarTypes.FirstOrDefault(t => t.Name == "Coupe");
            var stationWagonType = context.CarTypes.FirstOrDefault(t => t.Name == "Station Wagon");
            var convertibleType = context.CarTypes.FirstOrDefault(t => t.Name == "Convertible");
            var mpvType = context.CarTypes.FirstOrDefault(t => t.Name == "MPV");

            // Mevcut eski arabalar varsa silelim
            var existingCars = await context.Cars.ToListAsync();
            if (existingCars.Any())
            {
                context.Cars.RemoveRange(existingCars);
                await context.SaveChangesAsync();
            }

            // 21 araba ekle (güncel ve gerçekçi fiyatlarla)
            // Basit ve çalışan URL'ler kullanıyoruz
            context.Cars.AddRange(
                // Sedan kategorisi
                new Car
                {
                    Brand = "Mercedes", 
                    Model = "C180", 
                    Year = 2022, 
                    Color = "Siyah", 
                    Mileage = 15000, 
                    Price = 1850000M, 
                    EngineSize = "1.6", 
                    Description = "Hatasız boyasız Mercedes C180", 
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/blue-car-driving-road_114579-4056.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 6.5M,
                    Features = "Deri koltuk, Panoramik cam tavan, Navigasyon, Apple CarPlay",
                    CarTypeId = sedanType?.Id
                },
                new Car
                {
                    Brand = "BMW", 
                    Model = "320i", 
                    Year = 2021, 
                    Color = "Beyaz", 
                    Mileage = 25000, 
                    Price = 1680000M, 
                    EngineSize = "2.0", 
                    Description = "Tam donanım BMW 320i", 
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/white-premium-city-car-with-original-design-generative-ai_188544-8251.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 7.2M,
                    Features = "Hafıza koltuklar, Kablosuz şarj, LED farlar, Geri görüş kamerası",
                    CarTypeId = sedanType?.Id,
                    DiscountPercentage = 5M
                },
                new Car
                {
                    Brand = "Audi", 
                    Model = "A4", 
                    Year = 2021, 
                    Color = "Gri", 
                    Mileage = 10000, 
                    Price = 1750000M, 
                    EngineSize = "2.0", 
                    Description = "Audi A4 Quattro, 4x4 çekiş sistemi", 
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/blue-coupe-sport-car-highway-bridge-road_114579-4051.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 5.8M,
                    Features = "Quattro AWD, Matrix LED, B&O ses sistemi, Akıllı hız sabitleyici",
                    CarTypeId = sedanType?.Id
                },
                
                // Hatchback kategorisi
                new Car
                {
                    Brand = "Volkswagen",
                    Model = "Golf",
                    Year = 2022,
                    Color = "Kırmızı",
                    Mileage = 5000,
                    Price = 1250000M,
                    EngineSize = "1.5",
                    Description = "Volkswagen Golf 8 eTSI DSG",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/red-generic-suv-car-white-background-3d-illustration_101266-15530.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "Hatchback",
                    Doors = 5,
                    FuelEfficiency = 5.5M,
                    Features = "Dijital gösterge paneli, Elektrikli bagaj, Şerit takip sistemi",
                    CarTypeId = hatchbackType?.Id,
                    DiscountPercentage = 3M
                },
                new Car
                {
                    Brand = "Renault",
                    Model = "Clio",
                    Year = 2023,
                    Color = "Turuncu",
                    Mileage = 0,
                    Price = 950000M,
                    EngineSize = "1.0",
                    Description = "Yeni Renault Clio, sıfır kilometre",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/orange-hatchback-car-white-background_53876-89767.jpg",
                    FuelType = "Benzin",
                    Transmission = "Manuel",
                    BodyType = "Hatchback",
                    Doors = 5,
                    FuelEfficiency = 5.2M,
                    Features = "Apple CarPlay, Android Auto, Geri görüş kamerası, Akıllı fren sistemi",
                    CarTypeId = hatchbackType?.Id
                },
                new Car
                {
                    Brand = "Ford",
                    Model = "Focus",
                    Year = 2022,
                    Color = "Mavi",
                    Mileage = 8000,
                    Price = 1150000M,
                    EngineSize = "1.5",
                    Description = "Ford Focus ST-Line, sportif tasarım",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/blue-compact-suv-car-with-sport-modern-design-parked-road-3d-illustration_101266-15331.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "Hatchback",
                    Doors = 5,
                    FuelEfficiency = 4.8M,
                    Features = "ST-Line donanım, Spor koltuklar, Akıllı cruise control",
                    CarTypeId = hatchbackType?.Id
                },
                
                // Sedan (toyota, honda)
                new Car
                {
                    Brand = "Toyota",
                    Model = "Corolla",
                    Year = 2023,
                    Color = "Mavi",
                    Mileage = 0,
                    Price = 1180000M,
                    EngineSize = "1.8",
                    Description = "Toyota Corolla Hybrid, yakıt tasarruflu",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/silver-metallic-color-sport-sedan-road_114579-5035.jpg",
                    FuelType = "Hibrit",
                    Transmission = "CVT",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 4.2M,
                    Features = "Hibrit teknolojisi, Apple CarPlay, Geri görüş kamerası",
                    CarTypeId = sedanType?.Id
                },
                new Car
                {
                    Brand = "Honda",
                    Model = "Civic",
                    Year = 2022,
                    Color = "Gri",
                    Mileage = 20000,
                    Price = 1280000M,
                    EngineSize = "1.5",
                    Description = "Honda Civic Sedan Turbo, sportif tasarım",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/black-sedan-car-driving-road_114579-4039.jpg",
                    FuelType = "Benzin",
                    Transmission = "CVT",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 6.0M,
                    Features = "Honda Sensing, Apple CarPlay, Android Auto",
                    CarTypeId = sedanType?.Id,
                    DiscountPercentage = 7M
                },
                
                // SUV kategorisi
                new Car
                {
                    Brand = "Tesla",
                    Model = "Model Y",
                    Year = 2023,
                    Color = "Beyaz",
                    Mileage = 1000,
                    Price = 2650000M,
                    EngineSize = "Elektrik",
                    Description = "Tesla Model Y Performance, elektrikli SUV",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/white-suv-car-isolated-white-background_53876-137017.jpg",
                    FuelType = "Elektrik",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 0M,
                    Features = "Autopilot, Cam tavan, Premium iç mekan, Hızlı şarj",
                    CarTypeId = suvType?.Id
                },
                new Car
                {
                    Brand = "Range Rover",
                    Model = "Evoque",
                    Year = 2022,
                    Color = "Siyah",
                    Mileage = 8000,
                    Price = 2900000M,
                    EngineSize = "2.0",
                    Description = "Range Rover Evoque R-Dynamic, lüks SUV",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/luxury-offroad-vehicle-sunset_23-2150850418.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 6.3M,
                    Features = "Meridian ses sistemi, Terrain Response 2, 360 derece kamera",
                    CarTypeId = suvType?.Id
                },
                new Car
                {
                    Brand = "Audi",
                    Model = "Q5",
                    Year = 2021,
                    Color = "Beyaz",
                    Mileage = 15000,
                    Price = 2300000M,
                    EngineSize = "2.0",
                    Description = "Audi Q5 Quattro, lüks SUV",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/white-suv-car-road_114579-4014.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 6.8M,
                    Features = "Quattro AWD, LED farlar, Dijital gösterge paneli",
                    CarTypeId = suvType?.Id
                },
                
                // Spor - Coupe
                new Car
                {
                    Brand = "Porsche",
                    Model = "911",
                    Year = 2021,
                    Color = "Sarı",
                    Mileage = 3000,
                    Price = 8500000M,
                    EngineSize = "3.0",
                    Description = "Porsche 911 Carrera S, spor otomobil",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/yellow-sport-car-with-black-autotuning-road_114579-5051.jpg",
                    FuelType = "Benzin",
                    Transmission = "PDK",
                    BodyType = "Coupe",
                    Doors = 2,
                    FuelEfficiency = 8.9M,
                    Features = "Sport Chrono paketi, PASM, Spor egzoz, Burmester ses sistemi",
                    CarTypeId = coupeType?.Id
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "M4",
                    Year = 2023,
                    Color = "Yeşil",
                    Mileage = 2000,
                    Price = 5200000M,
                    EngineSize = "3.0",
                    Description = "BMW M4 Competition, yüksek performanslı spor araba",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/sports-car-driving-high-speed-highway_114579-4046.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "Coupe",
                    Doors = 2,
                    FuelEfficiency = 9.8M,
                    Features = "M Competition paketi, Karbon fiber detaylar, Harman Kardon ses sistemi",
                    CarTypeId = coupeType?.Id
                },
                
                // Kompakt SUV ve Crossover
                new Car
                {
                    Brand = "Hyundai",
                    Model = "Tucson",
                    Year = 2023,
                    Color = "Yeşil",
                    Mileage = 0,
                    Price = 1550000M,
                    EngineSize = "1.6",
                    Description = "Yeni Hyundai Tucson, hibrit teknolojisi",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/green-mini-suv-car-with-urban-background-3d-illustration_101266-15332.jpg",
                    FuelType = "Hibrit",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 5.7M,
                    Features = "Panoramik cam tavan, Kablosuz şarj, Gelişmiş sürücü yardım sistemleri",
                    CarTypeId = suvType?.Id
                },
                new Car
                {
                    Brand = "Kia",
                    Model = "Sportage",
                    Year = 2022,
                    Color = "Gri",
                    Mileage = 10000,
                    Price = 1450000M,
                    EngineSize = "1.6",
                    Description = "Kia Sportage, şık tasarımlı SUV",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/dark-gray-sport-utility-vehicle-road_114579-5044.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 6.2M,
                    Features = "Harman Kardon ses sistemi, Akıllı bagaj kapağı, Apple CarPlay",
                    CarTypeId = suvType?.Id,
                    DiscountPercentage = 4M
                },
                
                // Elektrikli kategorisi
                new Car
                {
                    Brand = "Tesla",
                    Model = "Model 3",
                    Year = 2023,
                    Color = "Beyaz",
                    Mileage = 1000,
                    Price = 2350000M,
                    EngineSize = "Elektrik",
                    Description = "Tesla Model 3 Performance, elektrikli sedan",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/sedan-white-car-isolated-white-background-3d-illustration_101266-15531.jpg",
                    FuelType = "Elektrik",
                    Transmission = "Otomatik",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 0M,
                    Features = "Autopilot, Cam tavan, Premium iç mekan, Hızlı şarj",
                    CarTypeId = sedanType?.Id
                },
                new Car
                {
                    Brand = "Volkswagen",
                    Model = "ID.4",
                    Year = 2023,
                    Color = "Mavi",
                    Mileage = 0,
                    Price = 2200000M,
                    EngineSize = "Elektrik",
                    Description = "Volkswagen ID.4, elektrikli SUV",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/blue-jeep-parking-public-zone_114579-4042.jpg",
                    FuelType = "Elektrik",
                    Transmission = "Otomatik",
                    BodyType = "SUV",
                    Doors = 5,
                    FuelEfficiency = 0M,
                    Features = "Dijital gösterge paneli, ID.Light, Otomatik park sistemi",
                    CarTypeId = suvType?.Id
                },
                
                // MPV kategorisi
                new Car
                {
                    Brand = "Volkswagen",
                    Model = "Caravelle",
                    Year = 2022,
                    Color = "Siyah",
                    Mileage = 5000,
                    Price = 2500000M,
                    EngineSize = "2.0",
                    Description = "Volkswagen Caravelle, geniş aileler için ideal araç",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/minivan-car-3d-vector-icon-simple-car-icon-illustration_634243-86.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "MPV",
                    Doors = 5,
                    FuelEfficiency = 7.5M,
                    Features = "9 koltuk, Elektrikli kayar kapılar, Adaptif hız sabitleyici",
                    CarTypeId = mpvType?.Id
                },
                
                // Ekonomik kategorisi
                new Car
                {
                    Brand = "Fiat",
                    Model = "Egea",
                    Year = 2022,
                    Color = "Beyaz",
                    Mileage = 12000,
                    Price = 750000M,
                    EngineSize = "1.4",
                    Description = "Fiat Egea, ekonomik sedan",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/white-car-isolated-white-background_53876-12096.jpg",
                    FuelType = "Benzin",
                    Transmission = "Manuel",
                    BodyType = "Sedan",
                    Doors = 4,
                    FuelEfficiency = 5.9M,
                    Features = "Apple CarPlay, Geri görüş kamerası, Yol bilgisayarı",
                    CarTypeId = sedanType?.Id,
                    DiscountPercentage = 5M
                },
                new Car
                {
                    Brand = "Dacia",
                    Model = "Sandero",
                    Year = 2022,
                    Color = "Mavi",
                    Mileage = 8000,
                    Price = 650000M,
                    EngineSize = "1.0",
                    Description = "Dacia Sandero, ekonomik hatchback",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/blue-hatchback-parked-public-parking-space_114579-4042.jpg",
                    FuelType = "Benzin",
                    Transmission = "Manuel",
                    BodyType = "Hatchback",
                    Doors = 5,
                    FuelEfficiency = 5.3M,
                    Features = "Yol bilgisayarı, Bluetooth, USB bağlantısı",
                    CarTypeId = hatchbackType?.Id,
                    DiscountPercentage = 3M
                },
                
                // Station Wagon
                new Car
                {
                    Brand = "Volvo",
                    Model = "V60",
                    Year = 2022,
                    Color = "Gri",
                    Mileage = 15000,
                    Price = 2100000M,
                    EngineSize = "2.0",
                    Description = "Volvo V60, güvenli ve geniş station wagon",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/front-view-silver-modern-car-parked-road_114579-5028.jpg",
                    FuelType = "Dizel",
                    Transmission = "Otomatik",
                    BodyType = "Station Wagon",
                    Doors = 5,
                    FuelEfficiency = 5.2M,
                    Features = "City Safety, 360 derece kamera, Harman Kardon ses sistemi",
                    CarTypeId = stationWagonType?.Id
                },
                
                // Convertible
                new Car
                {
                    Brand = "BMW",
                    Model = "430i",
                    Year = 2023,
                    Color = "Kırmızı",
                    Mileage = 0,
                    Price = 3850000M,
                    EngineSize = "2.0",
                    Description = "BMW 430i Convertible, açılabilir tavanlı",
                    IsAvailable = true,
                    ImageUrl = "https://img.freepik.com/free-photo/red-convertible-sports-car-driving-road_114579-4060.jpg",
                    FuelType = "Benzin",
                    Transmission = "Otomatik",
                    BodyType = "Convertible",
                    Doors = 2,
                    FuelEfficiency = 7.0M,
                    Features = "Deri koltuklar, Harman Kardon ses sistemi, Head-up display",
                    CarTypeId = convertibleType?.Id
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
