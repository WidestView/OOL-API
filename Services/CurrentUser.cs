using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<User?> GetCurrentUser(CancellationToken token = default)
        {
            var httpUser = _httpContext.HttpContext.User;

            var username = httpUser.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sid);

            if (username == null)
            {
                return null;
            }

            return await _context.Users.FindAsync(username.Value);
        }

        public async Task<Employee?> GetCurrentEmployee()
        {
            var user = await GetCurrentUser();

            if (user == null)
            {
                return null;
            }

            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Occupation)
                .FirstOrDefaultAsync(e => e.UserId == user.Cpf);

            return employee;
        }
    }
}
