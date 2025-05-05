using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using identityApp.Models;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IdentityContext _context;

    public HomeController(ILogger<HomeController> logger, IdentityContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Ana sayfada sadece mevcut ve satılık arabaları gösteriyoruz
        var featuredCars = await _context.Cars
            .Where(c => c.IsAvailable && !c.IsDeleted)
            .OrderByDescending(c => c.Year)
            .Take(6)  // Ana sayfada sadece 6 araba gösteriyoruz
            .ToListAsync();
            
        return View(featuredCars);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Hakkımızda sayfası
    public IActionResult About()
    {
        return View();
    }

    // İletişim sayfası
    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
