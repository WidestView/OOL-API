using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly CurrentUserInfo _currentUserInfo;
        private readonly OutputUserHandler _outputHandler;
        private readonly IPictureStorage<User, string> _pictureStorage;
        private readonly IPasswordHash _passwordHash;

        public UserController(
            CurrentUserInfo currentUserInfo,
            StudioContext context,
            IPictureStorage<User, string> pictureStorage,
            IPasswordHash passwordHash
        )
        {
            _currentUserInfo = currentUserInfo;
            _context = context;
            _pictureStorage = pictureStorage;
            _outputHandler = new OutputUserHandler();
            _passwordHash = passwordHash;
        }

        [HttpGet]
        [Route("greet")]
        public async Task<IActionResult> Greet(CancellationToken token = default)
        {
            var employee = await _currentUserInfo.GetCurrentEmployee(token);

            if (employee != null)
            {
                return Ok(_outputHandler.OutputFor(employee));
            }

            var user = await _currentUserInfo.GetCurrentUser(token);

            if (user != null)
            {
                return Ok(_outputHandler.OutputFor(user));
            }

            return Unauthorized();
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
        [AllowAnonymous]
        [SwaggerOperation("Registers a new user")]
        [SwaggerResponse(200, "The created User", typeof(OutputUser))]
        [SwaggerResponse(409, "Somethig went wrong on creating user")]

        public async Task<IActionResult> PostUser(InputUser inputUser, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            inputUser.HashPassword(_passwordHash);
            var user = inputUser.ToModel();

            if (await CpfExists(user.Cpf)) ModelState.AddModelError(nameof(user.Cpf), "User Cpf already in use");
            if (await EmailExists(user.Email)) ModelState.AddModelError(nameof(user.Email), "User Email already in use");

            if (!ModelState.IsValid) return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return Ok(_outputHandler.OutputFor(user));
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

        [AcceptVerbs("GET", "POST")]
        [Route("verifyemail")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (await EmailExists(email))
            {
                var result = $"Email {email} is already in use.";
                return Conflict(result);
            }

            return Ok(true);
        }

        [AcceptVerbs("GET", "POST")]
        [Route("verifycpf")]
        public async Task<IActionResult> VerifyCpf(string cpf)
        {
            if (await CpfExists(cpf))
            {
                var result = $"Cpf {cpf} is already in use.";
                return Conflict(result);
            }

            return Ok(true);
        }

        private async Task<bool> CpfExists(string cpf) => (await _context.Users.FirstOrDefaultAsync(user => user.Cpf == cpf)) != null;
        private async Task<bool> EmailExists(string email) => (await _context.Users.FirstOrDefaultAsync(user => user.Email == email)) != null;
    }
}
