using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoShootController : Controller
    {
        private readonly StudioContext _context;

        private readonly CurrentUserInfo _currentUser;

        public PhotoShootController(StudioContext context, CurrentUserInfo userInfo)
        {
            (_context, _currentUser) = (context, userInfo);
        }


        [HttpGet]
        public IActionResult ListAll()
        {
            return Json(
                _context.PhotoShoots.Select(shoot => new OutputPhotoShoot(shoot, false))
            );
        }

        [HttpGet]
        [Route("current")]
        public IActionResult ListCurrentPhotoshoots()
        {
            var employee = _currentUser.GetCurrentEmployee();

            if (employee == null) return Unauthorized();

            var shoots = _context.PhotoShoots.Where(
                shoot => shoot.Employees.Any(e => e.UserId == employee.UserId));

            return Ok(shoots.Select(s => new OutputPhotoShoot(s, false)));
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _context
                .PhotoShoots
                .Include(shot => shot.Images)
                .FirstOrDefault(shoot => shoot.ResourceId == id);

            if (result == null) return NotFound();

            return Json(new OutputPhotoShoot(result, true));
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] InputPhotoShoot input)
        {
            if (ModelState.IsValid)
            {
                var shot = input.ToPhotoShoot();

                if (_context.Orders.Find(shot.OrderId) == null) return NotFound();

                _context.PhotoShoots.Add(shot);

                _context.SaveChanges();

                var output = new OutputPhotoShoot(shot, true);

                return CreatedAtAction("GetById", new {id = shot.ResourceId}, output);
            }

            return new BadRequestObjectResult(ModelState);
        }
    }
}