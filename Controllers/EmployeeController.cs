using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly CurrentUserInfo _currentUserInfo;

        public EmployeeController(CurrentUserInfo currentUserInfo, StudioContext context)
        {
            _currentUserInfo = currentUserInfo;
            _context = context;
        }

        [HttpGet]
        [Route("info")]
        [SwaggerOperation("Retrieves the employee data from the current logged employee")]
        [SwaggerResponse(401, "There is no logged employee")]
        [SwaggerResponse(200, "The current logged employee", typeof(OutputEmployee))]
        public async Task<IActionResult> GetInfo()
        {
            var employee = await _currentUserInfo.GetCurrentEmployee();

            if (employee == null)
            {
                return Unauthorized();
            }

            return Ok(new OutputEmployee(employee));
        }

        [HttpGet]
        [SwaggerOperation("Lists employees")]
        [SwaggerResponse(200, "The current employees", typeof(IEnumerable<OutputEmployee>))]
        public async Task<IActionResult> ListEmployees()
        {
            var result = await _context.Employees
                .Include(employee => employee.User)
                .ToListAsync();

            return Ok(
                result
                    .Select(employee => new OutputEmployee(employee))
            );
        }
    }
}
