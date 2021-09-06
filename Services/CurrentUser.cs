using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using OOL_API.Data;
using OOL_API.Models;

#nullable enable

namespace OOL_API.Services
{
    public class CurrentUserInfo
    {
        private readonly StudioContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUserInfo(IHttpContextAccessor httpContext, StudioContext context)
        {
            _httpContext = httpContext;
            _context = context;
        }

        public User? GetCurrentUser()
        {
            var httpUser = _httpContext.HttpContext.User;

            var username = httpUser.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null) return null;

            return _context.Users.Find(username.Value);
        }

        public Employee? GetCurrentEmployee()
        {
            var user = GetCurrentUser();

            if (user == null) return null;

            var employee = _context.Employees
                .Include(e => e.User)
                .FirstOrDefault(e => e.UserId == user.Cpf);

            return employee;
        }
    }
}