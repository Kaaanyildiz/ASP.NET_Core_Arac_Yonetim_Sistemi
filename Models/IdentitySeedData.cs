using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Admin123"; // Daha basit bir şifre
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

            // Admin kullanıcısını bul
            var user = await userManager.FindByNameAsync(adminUser);
            
            // Eğer admin kullanıcısı varsa sil (her zaman yeni admin oluştur)
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }
            
            // Yeni admin kullanıcısı oluştur
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
                    ImageUrl = "https://resim.epey.com/2264/m_mercedes-c-180-156-ps-amg-21.jpg",
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
                    ImageUrl = "https://img.freepik.com/premium-photo/bmw-320i-is-compact-executive-car-manufactured-by-german-automaker-bmw_776859-775.jpg?w=1060",
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
                    ImageUrl = "https://images.unsplash.com/photo-1726003536800-b9ec0888cf36?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.pexels.com/photos/19109790/pexels-photo-19109790/free-photo-of-hizli-suratli-otomotiv-toprak-yol.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
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
                    ImageUrl = "https://images.unsplash.com/photo-1666335009171-3ddc17937d6d?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1625179298498-9deecbfa38cb?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1638618164682-12b986ec2a75?q=80&w=1887&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.pexels.com/photos/17654204/pexels-photo-17654204/free-photo-of-otoparkta-siyah-honda-civic.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
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
                    ImageUrl = "https://images.pexels.com/photos/15089585/pexels-photo-15089585/free-photo-of-araba-otomobil-arac-elektrik.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
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
                    ImageUrl = "https://arabam-blog.mncdn.com/wp-content/uploads/2021/02/range-rover-evoque-22-1068x602.jpg",
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
                    ImageUrl = "https://images.pexels.com/photos/20220997/pexels-photo-20220997/free-photo-of-araba-otomobil-arac-suv.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
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
                    ImageUrl = "https://images.unsplash.com/photo-1593353798398-6024b7444bb6?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1617531653332-bd46c24f2068?q=80&w=1830&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1575090536203-2a6193126514?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1649921777129-a28a26031a03?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://images.unsplash.com/photo-1585011664466-b7bbe92f34ef?q=80&w=1887&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                    ImageUrl = "https://img.freepik.com/premium-photo/silver-car-with-number-2-side_845403-1773.jpg?w=1380",
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
                    ImageUrl = "https://i.pinimg.com/736x/43/11/aa/4311aa91407b4bd10c55fead8f6c7f94.jpg",
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
                    ImageUrl = "https://i0.wp.com/bestsellingcarsblog.com/wp-content/uploads/2019/03/Fiat-Egea-Turkey-February-2019.jpg?w=600&ssl=1",
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
                    ImageUrl = "https://cdn.motor1.com/images/mgl/KbmWnq/s3/dacia-sandero-stepway-extreme-2023.webp",
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
                    ImageUrl = "https://v.wpimg.pl/ZTU1NS5qdTU0UzhgGgp4IHcLbDpcU3Z2IBN0cRpIaWwtBHxrGhx0NSBFIThAHS56JV1hJVodLDt4R3hjGBVueW0cK2YEFzlhNAV6YwIQaDFgBHt9XwE9dig",
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
                    ImageUrl = "https://img.freepik.com/premium-photo/white-convertible-car-with-brown-interior-is-parked-street-against-gray-wall_1302699-12.jpg?w=1060",
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
