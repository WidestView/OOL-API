using System;
using System.Collections.Generic;
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
        public IActionResult Login([FromBody] InputLogin login)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var user = AuthenticateUser(login!);

            if (user == null) return Unauthorized();

            var accessLevel = FindUserAccessLevel(user);

            var token = GenerateToken(user.Cpf!, accessLevel);

            return Ok(new {token});
        }


        [Authorize]
        [HttpGet]
        [Route("greet")]
        public IActionResult Greet()
        {
            var user = HttpContext.User;

            var username = user.Claims.FirstOrDefault(
                claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null) return Unauthorized();

            var userWithLogin = FindUserWithLogin(username.Value);

            if (userWithLogin == null) return Unauthorized();

            return Ok($"Hello, {userWithLogin.Name}");
        }

        [Authorize(Roles = AccessLevelInfo.SudoString)]
        [HttpGet]
        [Route("sudo-greet")]
        public IActionResult SudoGreet()
        {
            return Greet();
        }

        private User? AuthenticateUser(InputLogin login)
        {
            var user = FindUserWithLogin(login.Login);

            var hash = _passwordHash.Of(login.Password);

            if (user != null)
                if (user.Password != hash)
                    user = null;

            return user;
        }

        private string? FindUserAccessLevel(User user)
        {
            var employee = _context.Employees.Find(user.Cpf);

            if (employee == null)
                return null;
            return AccessLevelInfo.Strings[employee.AccessLevel];
        }

        private User? FindUserWithLogin(string login)
        {
            return _context.Users.FirstOrDefault(
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

            if (accessLevel != null) claims.Add(new Claim("roles", accessLevel));

            return claims;
        }
    }
}