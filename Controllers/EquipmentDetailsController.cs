using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<OutputEquipmentDetails>> ListDetails(CancellationToken token = default)
        {
            var result = await _context.EquipmentDetails
                .Where(details => !details.IsArchived).ToListAsync(token);

            var output = result.Select(
                async row => await _detailsHandler.OutputFor(row, token)
            );

            return await Task.WhenAll(output);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDetails(int id, CancellationToken token = default)
        {
            var result = await _context.EquipmentDetails.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            await _context.Entry(result).Collection(
                details => details.Equipments
            ).LoadAsync(token);

            await _context.Entry(result).Reference(
                details => details.Type
            ).LoadAsync(token);

            return Ok(await _detailsHandler.OutputFor(result, token));
        }

        [HttpPost]
        public async Task<IActionResult> AddEquipmentDetails([FromBody] InputEquipmentDetails input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var type = await _context.EquipmentTypes.FindAsync(input.TypeId);

            if (type == null)
            {
                return NotFound();
            }

            var details = input.ToModel();

            await _context.EquipmentDetails.AddAsync(details);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                actionName: nameof(GetDetails),
                routeValues: new {id = details.Id},
                value: _detailsHandler.OutputFor(details)
            );
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateDetails(int id, [FromBody] InputEquipmentDetails input)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(input);
            }

            var current = await _context.EquipmentDetails.FindAsync(id);

            if (current == null)
            {
                return NotFound();
            }

            var updated = input.ToModel();

            current.Name = updated.Name;
            current.Price = updated.Price;
            current.TypeId = updated.TypeId;

            await _context.SaveChangesAsync();

            return Ok(await _detailsHandler.OutputFor(current));
        }

        [HttpPost]
        [Route("image/{id}")]
        public async Task<IActionResult> PostImage(int id, [FromForm] IFormFile file, CancellationToken token = default)
        {
            if (!IPictureStorageInfo.IsSupported(file?.ContentType))
            {
                return StatusCode(400);
            }

            var entry = await _context.EquipmentDetails.FindAsync(id);

            if (entry == null)
            {
                return NotFound();
            }

            await using var stream = file!.OpenReadStream();

            await _pictureStorage.PostPicture(stream, entry);

            return Ok();
        }

        [HttpGet]
        [Route("image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var content = await _pictureStorage.GetPicture(id);

            if (content == null)
            {
                return NotFound();
            }

            return File(content, contentType: "image/jpeg");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            var entry = await _context.EquipmentDetails.FindAsync(id);

            if (entry != null)
            {
                entry.IsArchived = true;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
