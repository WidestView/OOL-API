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
    public class UsersController : ControllerBase
    {
        private readonly StudioContext _context;

        public UsersController(StudioContext context)
        {
            _context = context;
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/User/{id}/allImagesIds
        // TODO: REMOVE BECAUSE OF SECURITY RISKS
        [HttpGet("{id:int}/allImagesIds")]
        public async Task<ActionResult<IEnumerable<Image>>> GetAllImages(int id)
        {
            var images = _context.Images.Where(i => i.OwnerID == id).ToList();

            if (images == null)
            {
                return NotFound();
            }

            return images.Select(i => i.ID);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
