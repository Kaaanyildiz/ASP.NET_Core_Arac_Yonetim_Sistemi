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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Year,Color,Mileage,Price,EngineSize,FuelType,Transmission,BodyType,Doors,FuelEfficiency,Features,CarTypeId,IsAvailable,Description,ImageUrl,DiscountPercentage")] Car car)
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
    }
}