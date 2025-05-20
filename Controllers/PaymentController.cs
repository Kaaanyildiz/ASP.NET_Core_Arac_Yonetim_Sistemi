using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using identityApp.Models;
using Microsoft.EntityFrameworkCore;

namespace identityApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IdentityContext _context;

        public PaymentController(IdentityContext context)
        {
            _context = context;
        }

        // GET: Payment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Create Order
            var order = new Order
            {
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                TotalAmount = model.TotalAmount,
                Status = OrderStatus.Pending
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Simulate payment
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = model.TotalAmount,
                PaymentMethod = model.PaymentMethod
            };
            _context.Payments.Add(payment);
            order.Status = OrderStatus.Processing;
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { id = order.Id });
        }

        // GET: Payment/Success/5
        public async Task<IActionResult> Success(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }
    }
}
