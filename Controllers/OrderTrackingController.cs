using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using identityApp.Models;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly IdentityContext _context;

        public OrderTrackingController(IdentityContext context)
        {
            _context = context;
        }

        // GET: OrderTracking
        public IActionResult Index()
        {
            return View(new OrderTrackingViewModel());
        }

        // POST: OrderTracking/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrderTrackingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Orders = await _context.Orders
                .Where(o => o.CustomerEmail == model.CustomerEmail)
                .Include(o => o.Payments)
                .ToListAsync();

            return View(model);
        }
    }
}
