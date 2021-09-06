using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Models;
using OOL_API.Services;

#nullable enable

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly CurrentUserInfo _currentUserInfo;
        private readonly IPictureStorage<User, string> _pictureStorage;

        public UserController(
            CurrentUserInfo currentUserInfo,
            IPictureStorage<User, string> pictureStorage
        )
        {
            _currentUserInfo = currentUserInfo;
            _pictureStorage = pictureStorage;
        }

        [HttpGet]
        [Route("picture")]
        public IActionResult GetPicture()
        {
            var user = _currentUserInfo.GetCurrentUser();

            if (user == null) return Unauthorized();

            var content = _pictureStorage.GetPicture(user.Cpf);

            if (content == null) return NotFound();

            return File(content, "image/jpeg");
        }

        [HttpPost]
        [Route("upload-image")]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            var user = _currentUserInfo.GetCurrentUser();

            if (user == null) return Unauthorized();

            if (file.ContentType != "image/jpeg") return BadRequest();

            using var stream = file.OpenReadStream();

            _pictureStorage.PostPicture(stream, user);

            return CreatedAtAction("GetPicture", "");
        }
    }
}