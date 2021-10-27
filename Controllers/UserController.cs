using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetPicture(CancellationToken token = default)
        {
            var user = await _currentUserInfo.GetCurrentUser(token);

            if (user == null)
            {
                return Unauthorized();
            }

            var content = await _pictureStorage.GetPicture(user.Cpf, token);

            if (content == null)
            {
                return NotFound();
            }

            return File(content, contentType: "image/jpeg");
        }

        [HttpPost]
        [Route("upload-image")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var user = await _currentUserInfo.GetCurrentUser();

            if (user == null)
            {
                return Unauthorized();
            }

            if (file.ContentType != "image/jpeg")
            {
                return BadRequest();
            }

            await using var stream = file.OpenReadStream();

            await _pictureStorage.PostPicture(stream, user);

            return CreatedAtAction(actionName: "GetPicture", value: "");
        }
    }
}
