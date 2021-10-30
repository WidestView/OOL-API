using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly CurrentUserInfo _currentUserInfo;

        public EmployeeController(CurrentUserInfo currentUserInfo)
        {
            _currentUserInfo = currentUserInfo;
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
    }
}
