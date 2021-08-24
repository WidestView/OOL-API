using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly IPictureStorage<Package, int> _pictureStorage;

        public PackageController(StudioContext context, IPictureStorage<Package, int> pictureStorage)
        => (_context, _pictureStorage) = (context, pictureStorage);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Package>>> GetProducts()
        {
            return await _context.Packages.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetProduct(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        }

        [HttpGet("{id}/image")]
        public IActionResult GetImageContent(int id)
        {
            var content = _pictureStorage.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Package>> PostProduct(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = package.Id }, package);
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.Id == id);
        }
    }
}
