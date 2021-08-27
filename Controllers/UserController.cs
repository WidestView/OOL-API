using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly IAppSettings _settings;

        public UserController(IAppSettings settings, StudioContext context)
        {
            _settings = settings;
            _context = context;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] InputLogin login)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var user = AuthenticateEmployee(login!);

            if (user == null) return Unauthorized();

            string token = GenerateToken(user.UserId!);

            return Ok(new {token});
        }


        [Authorize]
        [Route("greet")]
        [HttpGet]
        public IActionResult Greet()
        {
            var user = HttpContext.User;

            var username = user.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null) return Unauthorized();

            var employee = FindEmployeeWithUsername(username.Value);

            if (employee == null) return Unauthorized();

            return Ok($"Hello, {employee.User.Name}");
        }

        private Employee? AuthenticateEmployee(InputLogin login)
        {
            var employee = FindEmployeeWithUsername(login.Username);

            if (employee != null)
                // todo: hash passwords
                if (employee.User.Password != login.Password)
                    employee = null;

            return employee;
        }

        private Employee? FindEmployeeWithUsername(string username)
        {
            var employee = _context.Employees
                .Include(it => it.User)
                .FirstOrDefault(it => it.UserId == username);

            return employee;
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