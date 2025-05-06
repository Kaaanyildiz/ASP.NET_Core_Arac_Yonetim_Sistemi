using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using identityApp.Models;
using Microsoft.AspNetCore.Authorization;
using identityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace identityApp.Controllers
{
    public class CarsController : Controller
    {
        private readonly IdentityContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CarsController(IdentityContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string brand, string model, string color, string fuelType, 
                                              string transmission, string bodyType, int? minYear, int? maxYear, 
                                              decimal? minPrice, decimal? maxPrice, string sortOrder, int? carTypeId, 
                                              bool showOnlyDiscounted = false)
        {
            ViewData["BrandSortParam"] = String.IsNullOrEmpty(sortOrder) ? "brand_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["YearSortParam"] = sortOrder == "Year" ? "year_desc" : "Year";
            ViewData["CurrentBrand"] = brand;
            ViewData["CurrentModel"] = model;
            ViewData["CurrentColor"] = color;
            ViewData["CurrentFuelType"] = fuelType;
            ViewData["CurrentTransmission"] = transmission;
            ViewData["CurrentBodyType"] = bodyType;
            ViewData["CurrentMinYear"] = minYear;
            ViewData["CurrentMaxYear"] = maxYear;
            ViewData["CurrentMinPrice"] = minPrice;
            ViewData["CurrentMaxPrice"] = maxPrice;
            ViewData["CurrentShowDiscounted"] = showOnlyDiscounted;
            ViewData["CurrentCarTypeId"] = carTypeId;

            // Marka listesi için SelectList
            ViewData["Brands"] = new SelectList(await _context.Cars
                .Where(c => !c.IsDeleted)
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync());

            // Yakıt tipi listesi için SelectList
            ViewData["FuelTypes"] = new SelectList(await _context.Cars
                .Where(c => !c.IsDeleted)
                .Select(c => c.FuelType)
                .Distinct()
                .OrderBy(ft => ft)
                .ToListAsync());

            // Şanzıman tipi listesi için SelectList  
            ViewData["Transmissions"] = new SelectList(await _context.Cars
                .Where(c => !c.IsDeleted)
                .Select(c => c.Transmission)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync());

            // Kasa tipi listesi için SelectList
            ViewData["BodyTypes"] = new SelectList(await _context.Cars
                .Where(c => !c.IsDeleted)
                .Select(c => c.BodyType)
                .Distinct()
                .OrderBy(bt => bt)
                .ToListAsync());
                
            // Araba kategorileri için SelectList
            ViewData["CarTypes"] = new SelectList(
                await _context.CarTypes.OrderBy(t => t.Name).ToListAsync(),
                "Id", "Name", carTypeId);

            // Filtreleme
            var cars = _context.Cars
                .Include(c => c.CarType)
                .Where(c => c.IsDeleted == false);

            if (!String.IsNullOrEmpty(brand))
            {
                cars = cars.Where(c => c.Brand.Contains(brand));
            }

            if (!String.IsNullOrEmpty(model))
            {
                cars = cars.Where(c => c.Model.Contains(model));
            }

            if (!String.IsNullOrEmpty(color))
            {
                cars = cars.Where(c => c.Color.Contains(color));
            }
            
            if (!String.IsNullOrEmpty(fuelType))
            {
                cars = cars.Where(c => c.FuelType == fuelType);
            }
            
            if (!String.IsNullOrEmpty(transmission))
            {
                cars = cars.Where(c => c.Transmission == transmission);
            }
            
            if (!String.IsNullOrEmpty(bodyType))
            {
                cars = cars.Where(c => c.BodyType == bodyType);
            }
            
            if (minYear.HasValue)
            {
                cars = cars.Where(c => c.Year >= minYear.Value);
            }
            
            if (maxYear.HasValue)
            {
                cars = cars.Where(c => c.Year <= maxYear.Value);
            }
            
            if (minPrice.HasValue)
            {
                cars = cars.Where(c => c.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                cars = cars.Where(c => c.Price <= maxPrice.Value);
            }
            
            if (showOnlyDiscounted)
            {
                cars = cars.Where(c => c.DiscountPercentage.HasValue && c.DiscountPercentage > 0);
            }
            
            if (carTypeId.HasValue)
            {
                cars = cars.Where(c => c.CarTypeId == carTypeId.Value);
            }

            // Sıralama
            switch (sortOrder)
            {
                case "brand_desc":
                    cars = cars.OrderByDescending(c => c.Brand);
                    break;
                case "Price":
                    cars = cars.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    cars = cars.OrderByDescending(c => c.Price);
                    break;
                case "Year":
                    cars = cars.OrderBy(c => c.Year);
                    break;
                case "year_desc":
                    cars = cars.OrderByDescending(c => c.Year);
                    break;
                default:
                    cars = cars.OrderBy(c => c.Brand);
                    break;
            }

            // Giriş yapan kullanıcı varsa, favori arabaları işaretlemek için gerekli veriyi ekleyelim
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favorites = await _context.FavoriteCars
                    .Where(fc => fc.UserId == userId)
                    .Select(fc => fc.CarId)
                    .ToListAsync();
                
                ViewData["FavoriteCarIds"] = favorites;
            }

            return View(await cars.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarType)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == false);
            
            if (car == null)
            {
                return NotFound();
            }

            // Giriş yapan kullanıcı varsa, favoride olup olmadığını kontrol edelim
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isFavorite = await _context.FavoriteCars
                    .AnyAsync(fc => fc.UserId == userId && fc.CarId == id);
                
                ViewData["IsFavorite"] = isFavorite;
            }

            return View(car);
        }

        // GET: Cars/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Araba kategorileri için SelectList
            ViewData["CarTypes"] = new SelectList(await _context.CarTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Brand,Model,Year,Color,Mileage,Price,EngineSize,FuelType,Transmission,BodyType,Doors,FuelEfficiency,Features,CarTypeId,Description,ImageUrl,DiscountPercentage")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Hata durumunda kategori listesini tekrar yükle
            ViewData["CarTypes"] = new SelectList(await _context.CarTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", car.CarTypeId);
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null || car.IsDeleted)
            {
                return NotFound();
            }
            
            // Araba kategorileri için SelectList 
            ViewData["CarTypes"] = new SelectList(await _context.CarTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", car.CarTypeId);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Year,Color,Mileage,Price,EngineSize,FuelType,Transmission,BodyType,Doors,FuelEfficiency,Features,CarTypeId,IsAvailable,Description,ImageUrl,DiscountPercentage")] Car car, bool updateImage = false)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Değiştirilmemesi gereken alanları koruyalım
                    car.IsDeleted = false;
                    car.DateAdded = await _context.Cars
                        .Where(c => c.Id == id)
                        .Select(c => c.DateAdded)
                        .FirstOrDefaultAsync();
                    
                    // Eğer otomatik resim atama isteği varsa        
                    if (updateImage)
                    {
                        TempData["Message"] = "Otomatik resim atama devre dışı bırakıldı. Lütfen araç resim URL'sini kendiniz belirleyin.";
                    }
                        
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            // Hata durumunda kategori listesini tekrar yükle
            ViewData["CarTypes"] = new SelectList(await _context.CarTypes.OrderBy(t => t.Name).ToListAsync(), "Id", "Name", car.CarTypeId);
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarType)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == false);
                
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                // Soft delete - veritabanından silmiyoruz, sadece görünmez yapıyoruz
                car.IsDeleted = true;
                car.IsAvailable = false;
                _context.Update(car);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Cars/ToggleAvailability/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null || car.IsDeleted)
            {
                return NotFound();
            }

            car.IsAvailable = !car.IsAvailable;
            _context.Update(car);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Cars/ToggleFavorite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null || car.IsDeleted)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorite = await _context.FavoriteCars
                .FirstOrDefaultAsync(fc => fc.UserId == userId && fc.CarId == id);
                
            if (favorite != null)
            {
                // Zaten favorilerde ise, favorilerden çıkar
                _context.FavoriteCars.Remove(favorite);
            }
            else
            {
                // Favorilerde değilse, favorilere ekle
                favorite = new FavoriteCar
                {
                    UserId = userId,
                    CarId = id,
                    DateAdded = DateTime.Now
                };
                _context.FavoriteCars.Add(favorite);
            }
            
            await _context.SaveChangesAsync();
            
            // Detay sayfasında ise, detay sayfasına geri dön
            if (Request.Headers["Referer"].ToString().Contains("/Cars/Details/"))
            {
                return RedirectToAction(nameof(Details), new { id });
            }
            
            // İndex sayfasına dön
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Cars/MyFavorites
        [Authorize]
        public async Task<IActionResult> MyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoriteCars = await _context.FavoriteCars
                .Where(fc => fc.UserId == userId)
                .Include(fc => fc.Car)
                .ThenInclude(c => c.CarType)
                .Where(fc => !fc.Car.IsDeleted)
                .OrderByDescending(fc => fc.DateAdded)
                .Select(fc => fc.Car)
                .ToListAsync();
                
            ViewData["FavoriteCarIds"] = favoriteCars.Select(c => c.Id).ToList();
            
            return View("Index", favoriteCars);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        // GET: Cars/UpdateAllImages
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateAllImages()
        {
            return View();
        }

        // POST: Cars/UpdateAllImagesConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAllImagesConfirmed()
        {
            var cars = await _context.Cars
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            int updatedCount = 0;

            foreach (var car in cars)
            {
                // Araba markası ve modeline göre uygun bir resim URL'si belirle
                string imageUrl = GetCarImageUrl(car.Brand, car.Model, car.BodyType);
                
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    car.ImageUrl = imageUrl;
                    _context.Update(car);
                    updatedCount++;
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{updatedCount} aracın resmi başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Araba markası ve modeline göre uygun resim URL'si döndüren yardımcı metot
        private string GetCarImageUrl(string brand, string model, string bodyType)
        {
            // Marka ve model bazlı resim URL'leri - Her araç için boş URL'ler
            Dictionary<string, Dictionary<string, string>> carImages = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "BMW", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "320i", "https://img.freepik.com/premium-photo/bmw-320i-is-compact-executive-car-manufactured-by-german-automaker-bmw_776859-775.jpg?w=1060" }, // BMW 320i için uygun resim URL'si
                        { "M4", "https://images.unsplash.com/photo-1617531653332-bd46c24f2068?q=80&w=1830&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // BMW M4 için uygun resim URL'si
                        { "430i", "https://img.freepik.com/premium-photo/white-convertible-car-with-brown-interior-is-parked-street-against-gray-wall_1302699-12.jpg?w=1060" }, // BMW 430i için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1588526562876-08b9ddfa14fe?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel BMW resmi
                    }
                },
                {
                    "Volkswagen", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Golf", "https://images.pexels.com/photos/19109790/pexels-photo-19109790/free-photo-of-hizli-suratli-otomotiv-toprak-yol.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // VW Golf için uygun resim URL'si
                        { "ID.4", "https://img.freepik.com/premium-photo/silver-car-with-number-2-side_845403-1773.jpg?w=1380" }, // VW ID.4 için uygun resim URL'si
                        { "Caravelle", "https://i.pinimg.com/736x/43/11/aa/4311aa91407b4bd10c55fead8f6c7f94.jpg" }, // VW Caravelle için uygun resim URL'si
                        { "DEFAULT", "https://img.freepik.com/free-photo/man-parked-side-road_23-2148321904.jpg?t=st=1746487450~exp=1746491050~hmac=d665a99fe9ea9ba7d581d15bd998ee52dbec1d8cbc391708cd1ab7cb99ec1274&w=1060" } // Genel VW resmi
                    }
                },
                {
                    "Mercedes", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "C180", "https://resim.epey.com/2264/m_mercedes-c-180-156-ps-amg-21.jpg" }, // Mercedes C180 için uygun resim URL'si
                        { "DEFAULT", "https://resim.epey.com/2264/m_mercedes-c-180-156-ps-amg-21.jpg" } // Genel Mercedes resmi
                    }
                },
                {
                    "Audi", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "A3", "https://images.pexels.com/photos/27810417/pexels-photo-27810417/free-photo-of-peyzaj-manzara-doga-gevseme.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // Audi A3 için uygun resim URL'si
                        { "A4", "https://images.unsplash.com/photo-1726003536800-b9ec0888cf36?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Audi A4 için uygun resim URL'si
                        { "A6", "https://images.pexels.com/photos/17888846/pexels-photo-17888846/free-photo-of-kent-sehir-sokak-arac.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // Audi A6 için uygun resim URL'si
                        { "Q5", "https://images.pexels.com/photos/20220997/pexels-photo-20220997/free-photo-of-araba-otomobil-arac-suv.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // Audi Q5 için uygun resim URL'si
                        { "DEFAULT", "https://images.pexels.com/photos/20220997/pexels-photo-20220997/free-photo-of-araba-otomobil-arac-suv.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" } // Genel Audi resmi
                    }
                },
                {
                    "Porsche", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "911", "https://images.unsplash.com/photo-1593353798398-6024b7444bb6?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Porsche 911 için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1593353798398-6024b7444bb6?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Porsche resmi
                    }
                },
                {
                    "Renault", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Clio", "https://images.unsplash.com/photo-1666335009171-3ddc17937d6d?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Renault Clio için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1666335009171-3ddc17937d6d?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Renault resmi
                    }
                },
                {
                    "Ford", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Focus", "https://images.unsplash.com/photo-1625179298498-9deecbfa38cb?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Ford Focus için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1625179298498-9deecbfa38cb?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Ford resmi
                    }
                },
                {
                    "Honda", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Civic", "https://images.pexels.com/photos/17654204/pexels-photo-17654204/free-photo-of-otoparkta-siyah-honda-civic.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // Honda Civic için uygun resim URL'si
                        { "DEFAULT", "https://images.pexels.com/photos/17654204/pexels-photo-17654204/free-photo-of-otoparkta-siyah-honda-civic.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" } // Genel Honda resmi
                    }
                },
                {
                    "Toyota", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Corolla", "https://images.unsplash.com/photo-1638618164682-12b986ec2a75?q=80&w=1887&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Toyota Corolla için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1638618164682-12b986ec2a75?q=80&w=1887&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Toyota resmi
                    }
                },
                {
                    "Tesla", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Model 3", "https://images.unsplash.com/photo-1585011664466-b7bbe92f34ef?q=80&w=1887&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Tesla Model 3 için uygun resim URL'si
                        { "Model Y", "https://images.pexels.com/photos/15089585/pexels-photo-15089585/free-photo-of-araba-otomobil-arac-elektrik.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" }, // Tesla Model Y için uygun resim URL'si
                        { "DEFAULT", "https://images.pexels.com/photos/15089585/pexels-photo-15089585/free-photo-of-araba-otomobil-arac-elektrik.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1" } // Genel Tesla resmi
                    }
                },
                {
                    "Range Rover", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Evoque", "https://arabam-blog.mncdn.com/wp-content/uploads/2021/02/range-rover-evoque-22-1068x602.jpg" }, // Range Rover Evoque için uygun resim URL'si
                        { "DEFAULT", "https://arabam-blog.mncdn.com/wp-content/uploads/2021/02/range-rover-evoque-22-1068x602.jpg" } // Genel Range Rover resmi
                    }
                },
                {
                    "Hyundai", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Tucson", "https://images.unsplash.com/photo-1575090536203-2a6193126514?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Hyundai Tucson için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1575090536203-2a6193126514?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Hyundai resmi
                    }
                },
                {
                    "Kia", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Sportage", "https://images.unsplash.com/photo-1649921777129-a28a26031a03?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Kia Sportage için uygun resim URL'si
                        { "DEFAULT", "https://images.unsplash.com/photo-1649921777129-a28a26031a03?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // Genel Kia resmi
                    }
                },
                {
                    "Fiat", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Egea", "https://i0.wp.com/bestsellingcarsblog.com/wp-content/uploads/2019/03/Fiat-Egea-Turkey-February-2019.jpg?w=600&ssl=1" }, // Fiat Egea için uygun resim URL'si
                        { "DEFAULT", "https://i0.wp.com/bestsellingcarsblog.com/wp-content/uploads/2019/03/Fiat-Egea-Turkey-February-2019.jpg?w=600&ssl=1" } // Genel Fiat resmi
                    }
                },
                {
                    "Dacia", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Sandero", "https://cdn.motor1.com/images/mgl/KbmWnq/s3/dacia-sandero-stepway-extreme-2023.webp" }, // Dacia Sandero için uygun resim URL'si
                        { "DEFAULT", "https://cdn.motor1.com/images/mgl/KbmWnq/s3/dacia-sandero-stepway-extreme-2023.webp" } // Genel Dacia resmi
                    }
                },
                {
                    "Volvo", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "V60", "https://v.wpimg.pl/ZTU1NS5qdTU0UzhgGgp4IHcLbDpcU3Z2IBN0cRpIaWwtBHxrGhx0NSBFIThAHS56JV1hJVodLDt4R3hjGBVueW0cK2YEFzlhNAV6YwIQaDFgBHt9XwE9dig" }, // Volvo V60 için uygun resim URL'si
                        { "DEFAULT", "https://v.wpimg.pl/ZTU1NS5qdTU0UzhgGgp4IHcLbDpcU3Z2IBN0cRpIaWwtBHxrGhx0NSBFIThAHS56JV1hJVodLDt4R3hjGBVueW0cK2YEFzlhNAV6YwIQaDFgBHt9XwE9dig" } // Genel Volvo resmi
                    }
                }
            };

            // Kasa tipine göre varsayılan resimler
            Dictionary<string, string> bodyTypeImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Sedan", "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Sedan resmi
                { "Hatchback", "https://images.unsplash.com/photo-1590510757557-e36176ba3cc8?q=80&w=1984&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Hatchback resmi
                { "SUV", "https://images.unsplash.com/photo-1601359169004-f9663c04c892?q=80&w=1964&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // SUV resmi
                { "Coupe", "https://images.unsplash.com/photo-1535732820275-9ffd998cac22?q=80&w=1936&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Coupe resmi
                { "Convertible", "https://images.unsplash.com/photo-1583267746897-2cf415887172?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Convertible resmi
                { "Station Wagon", "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }, // Station Wagon resmi
                { "MPV", "https://images.unsplash.com/photo-1596463716132-60592cd69db9?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" } // MPV resmi
            };

            // Öncelikle marka ve model eşleşmesine bak
            if (carImages.ContainsKey(brand))
            {
                var modelImages = carImages[brand];
                if (modelImages.ContainsKey(model) && !string.IsNullOrEmpty(modelImages[model]))
                {
                    return modelImages[model];
                }
                // Model bulunamadıysa, o marka için varsayılan resmi kullan
                else if (modelImages.ContainsKey("DEFAULT") && !string.IsNullOrEmpty(modelImages["DEFAULT"]))
                {
                    return modelImages["DEFAULT"];
                }
            }

            // Marka bulunamadıysa, kasa tipine göre varsayılan resmi kullan
            if (bodyTypeImages.ContainsKey(bodyType) && !string.IsNullOrEmpty(bodyTypeImages[bodyType]))
            {
                return bodyTypeImages[bodyType];
            }

            // Hiçbir eşleşme bulunamadıysa, boş string döndür
            return "";
        }
    }
}