using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly IPictureStorage<Package, int> _pictureStorage;

        public PackageController(StudioContext context, IPictureStorage<Package, int> pictureStorage)
        {
            (_context, _pictureStorage) = (context, pictureStorage);
        }

        [HttpGet]
        [SwaggerOperation("Lists all available photoshoot packages")]
        [SwaggerResponse(200, "The available packages", typeof(IEnumerable<Package>))]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _context.Packages.ToListAsync());
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Returns the package with the given ID")]
        [SwaggerResponse(200, "The package entry", typeof(Package))]
        [SwaggerResponse(404, "There was no package found with the given ID")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        [HttpGet("{id}/image")]
        [SwaggerOperation("Returns the image of the package with the given ID")]
        [SwaggerResponse(200, "The image of the package")]
        [SwaggerResponse(404, "No image was found for the package with the given ID")]
        public async Task<IActionResult> GetImageContent(int id)
        {
            var content = await _pictureStorage.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }

        [HttpPost]
        [SwaggerOperation("Registers a new package")]
        [SwaggerResponse(200, "The added package entry", typeof(Package))]
        public async Task<IActionResult> PostProduct(Package package)
        {
            await _context.Packages.AddAsync(package);

            await _context.SaveChangesAsync();

            return Ok(package);
        }
    }
}
