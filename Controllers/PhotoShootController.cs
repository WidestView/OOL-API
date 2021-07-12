using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Controllers
{
    [Route("photoshoot")]
    public class PhotoShootController : Controller
    {
        private readonly StudioContext _context;

        public PhotoShootController(StudioContext context)
            => _context = context;


        [HttpGet]
        public ActionResult<IEnumerable<PhotoShoot>> ListAll()
        {
            return _context.PhotoShoots;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _context
                .PhotoShoots
                .FirstOrDefault(shoot => shoot.ResourceId == id);

            if (result == null)
            {
                return NotFound();
            }

            return Json(result);
        }

        public class PhotoshootInput
        {
            [Required]
            public int OrderId { get; set; }

            [Required]
            public string Address { get; set; }

            [Required]
            public DateTime Start { get; set; }

            [Required]
            public uint DurationMinutes { get; set; }

            public PhotoShoot ToPhotoShoot()
            {
                return new PhotoShoot
                {
                    Address = Address,
                    Duration = TimeSpan.FromMinutes(DurationMinutes),
                    OrderId = OrderId,
                    Start = Start
                };
            } 
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] PhotoshootInput input)
        {
            if (ModelState.IsValid)
            {
                var shot = input.ToPhotoShoot();
                
                _context.PhotoShoots.Add(shot);

                _context.SaveChanges();

                return CreatedAtAction("GetById", new {id = shot.ResourceId}, shot);
            }

            return new BadRequestObjectResult(ModelState);
        }
    }
}