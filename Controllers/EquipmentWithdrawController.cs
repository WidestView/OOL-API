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

            _withdrawHandler = new OutputWithdrawHandler(context);

            _withdrawHandler
                .Bind(new OutputEquipmentHandler(context))
                .Bind(new OutputEquipmentDetailsHandler(context));
        }

        [HttpGet]
        public async Task<IActionResult> ListWithdraws(CancellationToken token)
        {
            var content = await _context.EquipmentWithDraws
                .Include(row => row.PhotoShoot)
                .Include(row => row.Employee)
                .Include(row => row.Employee.User)
                .Include(row => row.Equipment)
                .Include(row => row.Equipment.Details)
                .ToListAsync(token);

            var result = content
                .Select(async row => await _withdrawHandler.OutputFor(row, token))
                .ToList();

            return Ok(await Task.WhenAll(result));
        }
    }
}
