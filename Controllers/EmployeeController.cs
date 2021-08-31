using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Models;
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

        private readonly IPictureStorage<Employee, string> _pictureStorage;

        public EmployeeController(
            CurrentUserInfo currentUserInfo,
            IPictureStorage<Employee, string> pictureStorage
        )
        {
            _currentUserInfo = currentUserInfo;
            _pictureStorage = pictureStorage;
        }

        [HttpGet]
        [Route("picture")]
        public IActionResult GetPicture()
        {
            var employee = _currentUserInfo.GetCurrentEmployee();

            if (employee == null) return Unauthorized();

            var content = _pictureStorage.GetPicture(employee.UserId);

            if (content == null) return NotFound();

            return File(content, "image/jpeg");
        }

        [HttpPost]
        [Route("upload-image")]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            var employee = _currentUserInfo.GetCurrentEmployee();

            if (employee == null) return Unauthorized();

            if (file.ContentType != "image/jpeg") return BadRequest();

            using var stream = file.OpenReadStream();

            _pictureStorage.PostPicture(stream, employee);

            return CreatedAtAction("GetPicture", "");
        }

        [HttpGet]
        [Route("info")]
        public IActionResult GetInfo()
        {
            var employee = _currentUserInfo.GetCurrentEmployee();

            if (employee == null) return Unauthorized();

            return Ok(new OutputEmployee(employee));
        }
    }
}