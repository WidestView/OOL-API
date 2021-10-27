using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/equipment/withdraw")]
    public class EquipmentWithdrawController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly OutputWithdrawHandler _withdrawHandler;

        public EquipmentWithdrawController(StudioContext context)
        {
            _context = context;

            var equipmentHandler = new OutputEquipmentHandler(context);

            var withdrawHandler = new OutputWithdrawHandler(context)
            {
                EquipmentHandler = equipmentHandler
            };

            _withdrawHandler = withdrawHandler;
        }

        [HttpGet]
        public async Task<IActionResult> ListWithdraws(CancellationToken token)
        {
            var content = await _context.EquipmentWithDraws
                .Include(row => row.Employee)
                .Include(row => row.Equipment)
                .Include(row => row.PhotoShoot)
                .Include(row => row.Employee.User)
                .ToListAsync(token);

            var result = content
                .Select(async row => await _withdrawHandler.OutputFor(row, token))
                .ToList();

            return Ok(await Task.WhenAll(result));
        }
    }
}
