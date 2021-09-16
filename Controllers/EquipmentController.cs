using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly StudioContext _context;

        public EquipmentController(StudioContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("details")]
        public IEnumerable<OutputEquipmentDetails> ListDetails()
        {
            var result = _context.EquipmentDetails
                .Include(row => row.Type)
                .ToList();


            return result.Select(
                row => new OutputEquipmentDetails(row, true)
            );
        }

        [HttpGet]
        [Route("details/{id}")]
        public IActionResult GetDetails(int id)
        {
            var result = _context.EquipmentDetails.Find(id);

            if (result == null) return NotFound();

            _context.Entry(result).Collection(
                details => details.Equipments
            ).Load();

            _context.Entry(result).Reference(
                details => details.Type
            ).Load();

            return Ok(new OutputEquipmentDetails(result, true));
        }

        [HttpGet]
        public IEnumerable<OutputEquipment> ListEquipments()
        {
            var result = _context.Equipments
                .Include(row => row.Details)
                .Select(
                    row => new OutputEquipment(row, true)
                ).ToList();

            return result;
        }

        [HttpPost]
        [Route("add-details")]
        public IActionResult AddEquipmentDetails(InputEquipmentDetails input)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(input);

            var type = _context.EquipmentTypes.Find(input.TypeId);

            if (type == null) return NotFound();

            var details = input.ToModel();

            _context.EquipmentDetails.Add(details);

            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetDetails),
                new {id = details.Id},
                new OutputEquipmentDetails(details, true));
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddEquipment(InputEquipment input)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(input);

            var details = _context.EquipmentDetails.Find(input.DetailsId);

            if (details == null) return NotFound();

            var equipment = input.ToModel();
            equipment.Details = details;

            _context.Equipments.Add(equipment);

            _context.SaveChanges();

            return Ok(new OutputEquipment(equipment, true));
        }

        [HttpGet]
        [Route("types")]
        public IEnumerable<OutputEquipmentType> ListEquipmentTypes()
        {
            return _context.EquipmentTypes.Select(type => new OutputEquipmentType(type));
        }

        [HttpPost]
        [Route("add-type")]
        public IActionResult AddEquipmentType(InputEquipmentType input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            EquipmentType type = input.ToModel();

            _context.EquipmentTypes.Add(type);

            _context.SaveChanges();

            return Ok(new OutputEquipmentType(type));
        }
    }
}