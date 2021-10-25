using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private const OutputEquipmentDetails.Flags DetailsReferenceFlags =
            OutputEquipmentDetails.Flags.All ^ OutputEquipmentDetails.Flags.Equipments;

        private readonly StudioContext _context;

        public EquipmentController(StudioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<OutputEquipment> ListEquipments()
        {
            var result = _context.Equipments.ToList();

            foreach (var row in result)
            {
                _context.Entry(row).Reference(item => item.Details).Load();
                _context.Entry(row.Details).Reference(item => item.Type).Load();
            }

            return result.Select(row => new OutputEquipment(
                equipment: row,
                flags: OutputEquipment.Flags.All,
                detailsFlags: DetailsReferenceFlags));
        }

        [HttpPost]
        public IActionResult AddEquipment(InputEquipment input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var details = _context.EquipmentDetails.Find(input.DetailsId);

            if (details == null)
            {
                return NotFound();
            }

            var equipment = input.ToModel();
            equipment.Details = details;

            _context.Equipments.Add(equipment);

            _context.SaveChanges();

            return Ok(new OutputEquipment(equipment: equipment, flags: OutputEquipment.Flags.All,
                detailsFlags: DetailsReferenceFlags));
        }

        [HttpPut]
        public IActionResult UpdateEquipment(InputEquipment.ForUpdate input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var current = _context.Equipments.Find(input.Id);

            if (current == null)
            {
                return NotFound();
            }

            var updated = input.ToModel();

            current.Available = updated.Available;
            current.Details = updated.Details;
            current.DetailsId = updated.DetailsId;

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetEquipmentById(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null)
            {
                return NotFound();
            }

            _context.Entry(equipment).Reference(item => item.Details).Load();

            _context.Entry(equipment.Details).Reference(item => item.Type).Load();

            return Ok(new OutputEquipment(equipment: equipment, flags: OutputEquipment.Flags.All,
                detailsFlags: DetailsReferenceFlags));
        }

        [HttpGet]
        [Route("types")]
        public IEnumerable<OutputEquipmentType> ListEquipmentTypes()
        {
            return _context.EquipmentTypes.Select(type => new OutputEquipmentType(type));
        }

        [HttpPost]
        [Route("types")]
        public IActionResult AddEquipmentType(InputEquipmentType input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var type = input.ToModel();

            _context.EquipmentTypes.Add(type);

            _context.SaveChanges();

            return Ok(new OutputEquipmentType(type));
        }
    }
}
