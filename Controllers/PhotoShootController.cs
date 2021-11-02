using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoShootController : ControllerBase
    {
        private readonly StudioContext _context;

        private readonly CurrentUserInfo _currentUser;

        public PhotoShootController(StudioContext context, CurrentUserInfo userInfo)
        {
            (_context, _currentUser) = (context, userInfo);
        }


        [HttpGet]
        [SwaggerOperation("Lists all available photoshoots")]
        [SwaggerResponse(200, "The available photoshoots", typeof(IEnumerable<OutputPhotoShoot>))]
        public async Task<IActionResult> ListAll()
        {
            return Ok(
                await _context.PhotoShoots
                    .Select(shoot => new OutputPhotoShoot(shoot, false))
                    .ToListAsync()
            );
        }

        [HttpGet]
        [Route("current")]
        [SwaggerOperation("Lists all the photoshoots assigned to the current employee")]
        [SwaggerResponse(200, "The assigned photoshoots", typeof(IEnumerable<OutputPhotoShoot>))]
        [SwaggerResponse(401, "The employee is not authenticated", typeof(IEnumerable<OutputPhotoShoot>))]
        public async Task<IActionResult> ListCurrentPhotoshoots()
        {
            var employee = await _currentUser.GetCurrentEmployee();

            if (employee == null)
            {
                return Unauthorized();
            }

            var shoots = await _context.PhotoShoots.Where(
                    shoot => shoot.Employees.Any(e => e.UserId == employee.UserId))
                .ToListAsync();

            return Ok(shoots.Select(s => new OutputPhotoShoot(s, withReferences: false)));
        }


        [HttpGet("{id}")]
        [SwaggerOperation("Gets a photoshoot by its ID")]
        [SwaggerResponse(200, "The photoshoot", typeof(OutputPhotoShoot))]
        [SwaggerResponse(404, "No photoshoot was found with the given ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _context
                .PhotoShoots
                .Include(shot => shot.Images)
                .FirstOrDefaultAsync(shoot => shoot.ResourceId == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new OutputPhotoShoot(result, withReferences: true));
        }

        [HttpPost("add")]
        [SwaggerOperation("Inserts a photoshoot")]
        [SwaggerResponse(404, "Some of the required references were not found")]
        [SwaggerResponse(200, "The photoshoot entry")]
        public async Task<IActionResult> Add([FromBody] InputPhotoShoot input)
        {
            if (ModelState.IsValid)
            {
                var shot = input.ToPhotoShoot();

                if (await _context.Orders.FindAsync(shot.OrderId) == null)
                {
                    return NotFound();
                }

                await _context.PhotoShoots.AddAsync(shot);

                await _context.SaveChangesAsync();

                var output = new OutputPhotoShoot(shot, withReferences: true);

                return CreatedAtAction(actionName: "GetById", routeValues: new {id = shot.ResourceId}, output);
            }

            return new BadRequestObjectResult(ModelState);
        }
    }
}
