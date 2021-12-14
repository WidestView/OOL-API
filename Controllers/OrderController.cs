using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

namespace OOL_API.Controllers
{
#nullable enable

    [ApiController]
    [Route("order")]
    public class OrderController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly CurrentUserInfo _currentUser;

        private readonly OutputOrderHandler _handler;

        public OrderController(StudioContext context, CurrentUserInfo currentUser)
        {
            _context = context;
            _currentUser = currentUser;
            _handler = new OutputOrderHandler(_context)
            {
                PackageHandler = new OutputPackageHandler()
            };
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] InputOrder input)
        {
            var currentCustomer = await _currentUser.GetCurrentCustomer();

            if (currentCustomer == null)
            {
                return Unauthorized();
            }

            var package = await _context.Packages.FindAsync(input.PackageId);

            if (package == null)
            {
                return NotFound();
            }

            if (!IsValidQuantity(input.ImageQuantity, package))
            {
                return BadRequest("Invalid quantity");
            }

            var price = input.ImageQuantity * package.PricePerPhoto;

            Order order = input.ToModel(currentCustomer, package, price);

            await _context.Orders.AddAsync(order);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var currentCustomer = await _currentUser.GetCurrentCustomer();

            if (currentCustomer == null)
            {
                return Unauthorized();
            }

            var orders = await _context.Orders.Where(
                order => order.CustomerId == currentCustomer.UserId
            ).ToListAsync();

            var result = await Task.WhenAll(
                orders.Select(_handler.OutputFor)
            );

            return Ok(result);
        }

        private bool IsValidQuantity(int quantity, Package package)
        {
            if (quantity <= 0)
            {
                return false;
            }

            if (package.ImageQuantity == null)
            {
                return
                    quantity % package.QuantityMultiplier == 0 &&
                    quantity / package.QuantityMultiplier <= package.MaxIterations;
            }

            return quantity == package.ImageQuantity;
        }
    }
}
