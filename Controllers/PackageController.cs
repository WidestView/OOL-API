using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly StudioContext _context;

        public PackageController(StudioContext context)
        => _context = context;

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

        [HttpPost]
        public async Task<ActionResult<Package>> PostProduct(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = package.ID }, package);
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.ID == id);
        }
    }
}
