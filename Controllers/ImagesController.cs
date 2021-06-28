using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly StudioContext _context;

        public ImagesController(StudioContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Images/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            Image image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            try
            {
                var file = System.IO.File.OpenRead(_configuration.GetSection("ImagesBaseUrl").Get<string>() + image.File);
                return File(file, "image/jpeg");
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Images/{id}/data
        [HttpGet("{id:guid}/data")]
        public async Task<ActionResult<Image>> GetData(Guid id)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            return image;
        }

        private bool ImageExists(Guid id)
        {
            return _context.Images.Any(e => e.ID == id);
        }
    }
}
