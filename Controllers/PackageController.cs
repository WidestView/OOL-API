using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly OutputPackageHandler _packageHandler = new OutputPackageHandler();

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
            var result = await _context.Packages.ToListAsync();

            return Ok(result.Select(row => _packageHandler.OutputFor(row)));
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

            return Ok(_packageHandler.OutputFor(package));
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
        [SwaggerResponse(200, "The added package entry", typeof(OutputPackage))]
        public async Task<IActionResult> PostProduct(InputPackage input)
        {
            var package = input.ToModel();

            await _context.Packages.AddAsync(package);

            await _context.SaveChangesAsync();

            return Ok(_packageHandler.OutputFor(package));
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation("Updates a package")]
        [SwaggerResponse(200, "The updated package entry", typeof(OutputPackage))]
        [SwaggerResponse(404, "No existing package was found with the given ID")]
        public async Task<IActionResult> UpdateProduct(int id, InputPackage input)
        {
            var currentPackage = await _context.Packages.FindAsync(id);

            if (currentPackage == null) return NotFound();

            var newPackage = input.ToModel();

            currentPackage.Name = newPackage.Name;
            currentPackage.Description = newPackage.Description;
            currentPackage.BaseValue = newPackage.BaseValue;
            currentPackage.PricePerPhoto = newPackage.PricePerPhoto;
            currentPackage.ImageQuantity = newPackage.ImageQuantity;
            currentPackage.QuantityMultiplier = newPackage.QuantityMultiplier;
            currentPackage.QuantityMultiplier = newPackage.QuantityMultiplier;
            currentPackage.MaxIterations = newPackage.MaxIterations;
            currentPackage.Available = newPackage.Available;

            await _context.SaveChangesAsync();

            return Ok(_packageHandler.OutputFor(currentPackage));
        }
    }
}
