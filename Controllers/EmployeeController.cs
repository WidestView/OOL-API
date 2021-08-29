using Microsoft.AspNetCore.Mvc;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

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
        public IActionResult GetInfo()
        {
            var employee = _currentUserInfo.GetCurrentEmployee();

            if (employee == null) return Unauthorized();

            var output = new OutputEmployee
            {
                Cpf = employee.UserId,
                BirthDate = employee.User.BirthDate,
                Name = employee.User.Name,
                SocialName = employee.User.SocialName,
                Phone = employee.User.Phone,
                Email = employee.User.Email
            };

            return Ok(output);
        }
    }
}