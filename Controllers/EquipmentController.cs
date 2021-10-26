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
        public IEnumerable<OutputEquipment> ListEquipments()
        {
            var result = _context.Equipments.Where(equipment => !equipment.IsArchived).ToList();

            foreach (var row in result)
            {
                _context.Entry(row).Reference(item => item.Details).Load();
                _context.Entry(row.Details).Reference(item => item.Type).Load();
            }

            return result.Select(_equipmentHandler.OutputFor);
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

            return Ok(_equipmentHandler.OutputFor(equipment));
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateEquipment(int id, [FromBody] InputEquipment input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var current = _context.Equipments.Find(id);

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

            return Ok(_equipmentHandler.OutputFor(equipment));
        }

        [HttpGet]
        [Route("types")]
        public IEnumerable<OutputEquipmentType> ListEquipmentTypes()
        {
            return _context.EquipmentTypes
                .Where(type => !type.IsArchived)
                .Select(type => new OutputEquipmentType(type));
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
