using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        private readonly OutputEquipmentDetailsHandler _detailsHandler;

        private readonly IPictureStorage<EquipmentDetails, int> _pictureStorage;

        public EquipmentDetailsController(StudioContext context, IPictureStorage<EquipmentDetails, int> pictureStorage)
        {
            _context = context;
            _pictureStorage = pictureStorage;

            var detailsHandler = new OutputEquipmentDetailsHandler(context);

            var equipmentHandler = new OutputEquipmentHandler(context);

            detailsHandler.EquipmentHandler = equipmentHandler;
            equipmentHandler.DetailsHandler = detailsHandler;

            _detailsHandler = detailsHandler;
        }

        [HttpGet]
        public IEnumerable<OutputEquipmentDetails> ListDetails()
        {
            var result = _context.EquipmentDetails
                .Where(details => !details.IsArchived).ToList();

            return result.Select(_detailsHandler.OutputFor);
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

            return Ok(_detailsHandler.OutputFor(result));
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
                value: _detailsHandler.OutputFor(details)
            );
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

            using var stream = file!.OpenReadStream();

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

        [HttpGet]
        [Route("archive/{id}")]
        public IActionResult Archive(int id)
        {
            var entry = _context.EquipmentDetails.Find(id);

            if (entry == null)
            {
                return NotFound();
            }

            entry.IsArchived = true;

            return Ok();
        }
    }
}
