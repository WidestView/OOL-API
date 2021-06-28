using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly StudioContext _context;

        public ImagesController(StudioContext context)
        {
            _context = context;
        }

        // GET: api/Images/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetImage(Guid id)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            return image; //TODO: MAKE IT RETURN THE IMAGE FILE
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
