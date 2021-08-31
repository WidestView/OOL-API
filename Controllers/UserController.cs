using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

#nullable enable

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly IPasswordHash _passwordHash;
        private readonly IAppSettings _settings;

        public UserController(IAppSettings settings, StudioContext context, IPasswordHash passwordHash)
        {
            _settings = settings;
            _context = context;
            _passwordHash = passwordHash;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] InputLogin login)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var user = AuthenticateUser(login!);

            if (user == null) return Unauthorized();

            string token = GenerateToken(user.Cpf!);

            return Ok(new {token});
        }


        [Authorize]
        [HttpGet]
        [Route("greet")]
        public IActionResult Greet()
        {
            var user = HttpContext.User;

            var username = user.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null) return Unauthorized();

            var userWithLogin = FindUserWithLogin(username.Value);

            if (userWithLogin == null) return Unauthorized();

            return Ok($"Hello, {userWithLogin.Name}");
        }

        private User? AuthenticateUser(InputLogin login)
        {
            var employee = FindUserWithLogin(login.Login);

            var hash = _passwordHash.Of(login.Password);

            if (employee != null)
                if (employee.Password != hash)
                    employee = null;

            return employee;
        }

        private User? FindUserWithLogin(string login)
        {
            return _context.Users.FirstOrDefault(
                user => user.Email == login || user.Cpf == login
            );
        }

        private string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));

            var issuer = _settings.JwtIssuer;

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, username)
            };

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}