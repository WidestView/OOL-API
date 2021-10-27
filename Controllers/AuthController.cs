using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

#nullable enable

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class AuthController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly IPasswordHash _passwordHash;
        private readonly IAppSettings _settings;

        public AuthController(IAppSettings settings, StudioContext context, IPasswordHash passwordHash)
        {
            _settings = settings;
            _context = context;
            _passwordHash = passwordHash;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] InputLogin login)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var user = await AuthenticateUser(login!);

            if (user == null)
            {
                return Unauthorized();
            }

            var accessLevel = await FindUserAccessLevel(user);

            var token = GenerateToken(username: user.Cpf!, accessLevel);

            return Ok(new {token});
        }


        [Authorize]
        [HttpGet]
        [Route("greet")]
        public async Task<IActionResult> Greet()
        {
            var user = HttpContext.User;

            var username = user.Claims.FirstOrDefault(
                claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null)
            {
                return Unauthorized();
            }

            var userWithLogin = await FindUserWithLogin(username.Value);

            if (userWithLogin == null)
            {
                return Unauthorized();
            }

            return Ok($"Hello, {userWithLogin.Name}");
        }

        [Authorize(Roles = AccessLevelInfo.SudoString)]
        [HttpGet]
        [Route("sudo-greet")]
        public Task<IActionResult> SudoGreet()
        {
            return Greet();
        }

        private async Task<User?> AuthenticateUser(InputLogin login)
        {
            var user = await FindUserWithLogin(login.Login);

            var hash = _passwordHash.Of(login.Password);

            if (user != null)
            {
                if (user.Password != hash)
                {
                    user = null;
                }
            }

            return user;
        }

        private async Task<string?> FindUserAccessLevel(User user)
        {
            var employee = await _context.Employees.FindAsync(user.Cpf);

            if (employee == null)
            {
                return null;
            }

            return AccessLevelInfo.Strings[employee.AccessLevel];
        }

        private async Task<User?> FindUserWithLogin(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(
                user => user.Email == login || user.Cpf == login
            );
        }

        private string GenerateToken(string username, string? accessLevel = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));

            var issuer = _settings.JwtIssuer;

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = GetClaims(username, accessLevel);

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static IEnumerable<Claim> GetClaims(string username, string? accessLevel)
        {
            var claims = new List<Claim>(2)
            {
                new Claim(JwtRegisteredClaimNames.Sid, username)
            };

            if (accessLevel != null)
            {
                claims.Add(new Claim(type: "roles", accessLevel));
            }

            return claims;
        }
    }
}
