using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly OutputEquipmentHandler _equipmentHandler;

        public EquipmentController(StudioContext context)
        {
            _context = context;

            var detailsHandler = new OutputEquipmentDetailsHandler(context);
            var equipmentHandler = new OutputEquipmentHandler(context);

            detailsHandler.EquipmentHandler = equipmentHandler;
            equipmentHandler.DetailsHandler = detailsHandler;

            _equipmentHandler = equipmentHandler;
        }

        [HttpGet]
        public async Task<IEnumerable<OutputEquipment>> ListEquipments(CancellationToken token = default)
        {
            var result = await _context.Equipments
                .Where(equipment => !equipment.IsArchived)
                .ToListAsync(token);

            foreach (var row in result)
            {
                await _context.Entry(row)
                    .Reference(item => item.Details)
                    .LoadAsync(token);

                await _context.Entry(row.Details)
                    .Reference(item => item.Type)
                    .LoadAsync(token);
            }

            return await Task.WhenAll(
                result.Select(async row => await _equipmentHandler.OutputFor(row, token))
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddEquipment(InputEquipment input)
        {
            var details = await _context.EquipmentDetails.FindAsync(input.DetailsId);

            if (details == null)
            {
                return NotFound();
            }

            var equipment = input.ToModel();
            equipment.Details = details;

            await _context.Equipments.AddAsync(equipment);

            await _context.SaveChangesAsync();

            return Ok(_equipmentHandler.OutputFor(equipment));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] InputEquipment input)
        {
            var current = await _context.Equipments.FindAsync(id);

            if (current == null)
            {
                return NotFound();
            }

            var updated = input.ToModel();

            current.Available = updated.Available;
            current.DetailsId = updated.DetailsId;

            await _context.SaveChangesAsync();

            return Ok(await _equipmentHandler.OutputFor(current));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetEquipmentById(int id, CancellationToken token)
        {
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            await _context.Entry(equipment)
                .Reference(item => item.Details)
                .LoadAsync(token);

            await _context.Entry(equipment.Details)
                .Reference(item => item.Type)
                .LoadAsync(token);

            return Ok(await _equipmentHandler.OutputFor(equipment, token));
        }

        [HttpGet]
        [Route("types")]
        public async Task<IEnumerable<OutputEquipmentType>> ListEquipmentTypes(CancellationToken token = default)
        {
            return await _context.EquipmentTypes
                .Where(type => !type.IsArchived)
                .Select(type => new OutputEquipmentType(type))
                .ToListAsync(token);
        }

        [HttpPost]
        [Route("types")]
        public async Task<IActionResult> AddEquipmentType(InputEquipmentType input)
        {
            var type = input.ToModel();

            await _context.EquipmentTypes.AddAsync(type);

            await _context.SaveChangesAsync();

            return Ok(new OutputEquipmentType(type));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            var entry = await _context.Equipments.FindAsync(id);

            if (entry != null)
            {
                entry.IsArchived = true;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
