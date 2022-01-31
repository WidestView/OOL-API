using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/[controller]")]
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
            var currentCustomer = await _currentUser.GetCurrentUser();

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

            var price = CalculatePrice(package, input.ImageQuantity);

            Order order = input.ToModel(currentCustomer, package, price);

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

            return Ok(await _handler.OutputFor(order));
        }

        private static decimal CalculatePrice(Package package, int inputImageQuantity)
        {
            return package.BaseValue + inputImageQuantity * package.PricePerPhoto;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> ListAll()
        {
            var orders = await _context.Orders.ToListAsync();

            var result = await Task.WhenAll(
                orders.Select(_handler.OutputFor)
            );

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var currentCustomer = await _currentUser.GetCurrentUser();

            if (currentCustomer == null)
            {
                return Unauthorized();
            }

            var orders = await _context.Orders.Where(
                order => order.CustomerId == currentCustomer.Cpf
            ).ToListAsync();

            var result = await Task.WhenAll(
                orders.Select(_handler.OutputFor)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var result = await _handler.OutputFor(order);

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
