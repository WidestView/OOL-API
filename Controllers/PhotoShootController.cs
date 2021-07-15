using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;

namespace OOL_API.Controllers
{
    [Route("photoshoot")]
    public class PhotoShootController : Controller
    {
        private readonly StudioContext _context;

        public PhotoShootController(StudioContext context)
            => _context = context;


        [HttpGet]
        public IActionResult ListAll()
        {
            return Json(
                _context.PhotoShoots.Select(shoot => new OutputPhotoShoot(shoot, false))
            );
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _context
                .PhotoShoots
                .Include(shot => shot.Images)
                .FirstOrDefault(shoot => shoot.ResourceId == id);

            if (result == null)
            {
                return NotFound();
            }

            return Json(new OutputPhotoShoot(result, true));
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] InputPhotoShoot input)
        {
            if (ModelState.IsValid)
            {
                var shot = input.ToPhotoShoot();

                _context.PhotoShoots.Add(shot);

                _context.SaveChanges();

                var output = new OutputPhotoShoot(shot, true);

                return CreatedAtAction("GetById", new {id = shot.ResourceId}, output);
            }

            return new BadRequestObjectResult(ModelState);
        }
    }
}