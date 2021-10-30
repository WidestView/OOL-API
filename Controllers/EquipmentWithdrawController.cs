using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation("Lists the current withdraws")]
        [SwaggerResponse(200, "The available withdraws", typeof(IEnumerable<OutputWithdraw>))]
        public async Task<IActionResult> ListWithdraws(CancellationToken token)
        {
            var content = await _context.EquipmentWithdraws
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

        [HttpPost]
        [SwaggerOperation("Registers the given equipment withdraw")]
        [SwaggerResponse(200, "The entry of the given withdraw", typeof(OutputWithdraw))]
        public async Task<IActionResult> AddWithdraw(InputWithdraw input)
        {
            var references = await FindReferences(input);

            if (references == null)
            {
                return NotFound();
            }

            var withdraw = input.ToModel(references.Value);

            await _context.EquipmentWithdraws.AddAsync(withdraw);

            await _context.SaveChangesAsync();

            return Ok(await _withdrawHandler.OutputFor(withdraw));
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation("Updates the given input withdraw")]
        [SwaggerResponse(200, "The current entry of the given withdraw", typeof(OutputWithdraw))]
        [SwaggerResponse(404, "Some of the given id references was invalid")]
        public async Task<IActionResult> UpdateWithdraw(int id, InputWithdraw input)
        {
            var entry = await _context.EquipmentWithdraws.FindAsync(id);

            if (entry == null)
            {
                return NotFound();
            }

            var references = await FindReferences(input);

            if (references == null)
            {
                return NotFound();
            }

            var updated = input.ToModel(references.Value);

            entry.WithdrawDate = updated.WithdrawDate;
            entry.ExpectedDevolutionDate = updated.ExpectedDevolutionDate;
            entry.EffectiveDevolutionDate = updated.EffectiveDevolutionDate;
            entry.PhotoShootId = updated.PhotoShootId;
            entry.PhotoShoot = updated.PhotoShoot;
            entry.EmployeeCpf = updated.EmployeeCpf;
            entry.Employee = updated.Employee;
            entry.EquipmentId = updated.EquipmentId;
            entry.Equipment = updated.Equipment;

            await _context.SaveChangesAsync();

            return Ok(await _withdrawHandler.OutputFor(entry));
        }

        private async Task<(Employee, Equipment, PhotoShoot)?> FindReferences(InputWithdraw input)
        {
            var employee = await _context.Employees.FindAsync(input.EmployeeCpf);

            if (employee == null)
            {
                return null;
            }

            var equipment = await _context.Equipments.FindAsync(input.EquipmentId);

            if (equipment == null)
            {
                return null;
            }

            var shoot = await
                _context
                    .PhotoShoots
                    .FirstOrDefaultAsync(row => row.ResourceId == input.PhotoShootId);

            if (shoot == null)
            {
                return null;
            }

            return (employee, equipment, shoot);
        }
    }
}
