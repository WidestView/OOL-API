using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/equipment/details")]
    public class EquipmentDetailsController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly IPictureStorage<EquipmentDetails, int> _pictureStorage;

        public EquipmentDetailsController(StudioContext context, IPictureStorage<EquipmentDetails, int> pictureStorage)
        {
            _context = context;
            _pictureStorage = pictureStorage;
        }

        [HttpGet]
        public IEnumerable<OutputEquipmentDetails> ListDetails()
        {
            var result = _context.EquipmentDetails
                .Include(row => row.Type)
                .ToList();


            return result.Select(
                row => new OutputEquipmentDetails(details: row, flags: OutputEquipmentDetails.Flags.All)
            );
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetDetails(int id)
        {
            var result = _context.EquipmentDetails.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            _context.Entry(result).Collection(
                details => details.Equipments
            ).Load();

            _context.Entry(result).Reference(
                details => details.Type
            ).Load();

            return Ok(new OutputEquipmentDetails(details: result, flags: OutputEquipmentDetails.Flags.All));
        }

        [HttpPost]
        public IActionResult AddEquipmentDetails([FromBody] InputEquipmentDetails input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var type = _context.EquipmentTypes.Find(input.TypeId);

            if (type == null)
            {
                return NotFound();
            }

            var details = input.ToModel();

            _context.EquipmentDetails.Add(details);

            _context.SaveChanges();

            return CreatedAtAction(
                actionName: nameof(GetDetails),
                routeValues: new {id = details.Id},
                value: new OutputEquipmentDetails(details: details, flags: OutputEquipmentDetails.Flags.All));
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateDetails(int id, [FromBody] InputEquipmentDetails input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var entry = _context.EquipmentDetails.Find(id);

            if (entry == null)
            {
                return NotFound();
            }

            var updated = input.ToModel();

            input.Name = updated.Name;
            input.Price = updated.Price;
            input.TypeId = updated.TypeId;

            _context.SaveChanges();

            return Ok(entry);
        }

        [HttpPost]
        [Route("image/{id}")]
        public IActionResult PostImage(int id, [FromForm] IFormFile file)
        {
            if (!IPictureStorageInfo.IsSupported(file?.ContentType))
            {
                return StatusCode(400);
            }

            var entry = _context.EquipmentDetails.Find(id);

            if (entry == null)
            {
                return NotFound();
            }

            using var stream = file.OpenReadStream();

            _pictureStorage.PostPicture(stream: stream, model: entry);

            return Ok();
        }

        [HttpGet]
        [Route("image/{id}")]
        public IActionResult GetImage(int id)
        {
            var content = _pictureStorage.GetPicture(id);

            if (content == null)
            {
                return NotFound();
            }

            return File(fileContents: content, contentType: "image/jpeg");
        }
    }
}
