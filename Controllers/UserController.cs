using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OOL_API.Models.DataTransfer;

#nullable enable

namespace OOL_API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] InputLogin login)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var user = AuthenticateUser(login!);

            if (user == null) return Unauthorized();

            string token = GenerateToken(user.Username!);

            return Ok(new {token});
        }


        [Authorize]
        [Route("greet")]
        public IActionResult Greet()
        {
            var user = HttpContext.User;

            var usernameClaim = user.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (usernameClaim != null) return Ok($"Hello, {usernameClaim.Value}");

            return Unauthorized();
        }

        private InputLogin? AuthenticateUser(InputLogin login)
        {
            // todo: return user model
            // todo: add database verification

            if (login.Username == "beep" && login.Password == "boop")
                return login;
            return null;
        }

        private string GenerateToken(string username)
        {
            var configIssuer = _configuration["Jwt:Issuer"] ?? throw new KeyNotFoundException("Missing Jwt Issuer");

            var configKey = _configuration["Jwt:Key"] ?? throw new KeyNotFoundException("Missing Jwt Key");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, username)
            };

            var token = new JwtSecurityToken(
                configIssuer,
                configIssuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}