using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase

    {
        private readonly OutputAccessLevelHandler _accessLevelHandler;
        private readonly StudioContext _context;
        private readonly CurrentUserInfo _currentUserInfo;
        private readonly IPasswordHash _passwordHash;

        public EmployeeController(
            CurrentUserInfo currentUserInfo,
            StudioContext context,
            IPasswordHash passwordHash
        )
        {
            _currentUserInfo = currentUserInfo;
            _context = context;
            _passwordHash = passwordHash;
            _accessLevelHandler = new OutputAccessLevelHandler();
        }

        [HttpGet]
        [Route("info")]
        [SwaggerOperation("Retrieves the employee data from the current logged employee")]
        [SwaggerResponse(401, "There is no logged employee")]
        [SwaggerResponse(200, "The current logged employee", typeof(OutputEmployee))]
        public async Task<IActionResult> GetInfo()
        {
            var employee = await _currentUserInfo.GetCurrentEmployee();

            if (employee == null)
            {
                return Unauthorized();
            }

            return Ok(new OutputEmployee(employee));
        }

        [HttpGet]
        [SwaggerOperation("Lists employees")]
        [SwaggerResponse(200, "The current employees", typeof(IEnumerable<OutputEmployee>))]
        public async Task<IActionResult> ListEmployees()
        {
            var result = await _context.Employees
                .Include(employee => employee.Occupation)
                .Include(employee => employee.User)
                .Where(row => row.User.Active)
                .ToListAsync();

            return Ok(
                result
                    .Select(employee => new OutputEmployee(employee))
            );
        }

        [HttpPut]
        [Authorize(Roles = AccessLevelInfo.SudoString)]
        [SwaggerOperation(
            Summary = "Updates the given employee entry",
            Description = "This operation requires high level permissions")
        ]
        [SwaggerResponse(200, "The updated entry", typeof(OutputEmployee))]
        [SwaggerResponse(401, "The employee was not authenticated")]
        [SwaggerResponse(404, "Some of the required references were not found")]
        public async Task<IActionResult> UpdateGiven([FromBody] InputEmployee input)
        {
            var currentEmployee = await _currentUserInfo.GetCurrentEmployee();

            if (currentEmployee == null)
            {
                return Unauthorized();
            }

            if (input.Cpf == currentEmployee.UserId)
            {
                ModelState.AddModelError("", "Self updates must be done through the " +
                                             "/api/employee/info endpoint");

                return BadRequest(ModelState);
            }

            return await UpdateEntry(input);
        }

        [HttpPut]
        [Route("info")]
        [SwaggerOperation("Updates the entry of the current employee")]
        [SwaggerResponse(200, "The updated entry", typeof(OutputEmployee))]
        [SwaggerResponse(401, "The employee was not authenticated")]
        [SwaggerResponse(404, "Some of the required references were not found")]
        public async Task<IActionResult> UpdateSelf([FromBody] InputEmployee input)
        {
            var currentEmployee = await _currentUserInfo.GetCurrentEmployee();

            if (currentEmployee == null)
            {
                return Unauthorized();
            }

            if (input.Cpf != currentEmployee.UserId)
            {
                ModelState.AddModelError("", "Cpf changes are not allowed");

                return BadRequest(ModelState);
            }

            if (input.AccessLevel != currentEmployee.AccessLevel)
            {
                ModelState.AddModelError("", "Self access level changes are not allowed");

                return BadRequest(ModelState);
            }

            return await UpdateEntry(input);
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Fetches the employee with the given id")]
        [SwaggerResponse(200, "The entry", typeof(OutputEmployee))]
        [SwaggerResponse(404, "No employee with the given ID was found")]
        public async Task<IActionResult> GetEmployee(string id)
        {
            var cpf = Misc.StripCpf(id);

            var result = await _context.Employees
                .Include(row => row.User)
                .Include(row => row.Occupation)
                .FirstOrDefaultAsync(row => row.UserId == cpf);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new OutputEmployee(result));
        }

        [HttpPost]
        [Authorize(Roles = AccessLevelInfo.SudoString)]
        [SwaggerOperation("Registers a new employee")]
        [SwaggerResponse(200, "The added entry", typeof(OutputEmployee))]
        [SwaggerResponse(401, "The employee is not authorize / authenticated")]
        [SwaggerResponse(404, "Some of the required references were not found")]
        [SwaggerResponse(409, "The CPF already exists")]
        public async Task<IActionResult> AddEmployee([FromBody] InputEmployee input)
        {
            var existing = await _context.Employees.FindAsync(Misc.StripCpf(input.Cpf));

            var exists = existing != null;

            if (exists)
            {
                ModelState.AddModelError(nameof(InputEmployee.Cpf), "CPF já existente");

                return Conflict(ModelState);
            }

            var occupation = await _context.Occupations.FindAsync(input.OccupationId);

            if (occupation == null)
            {
                ModelState.AddModelError(nameof(InputEmployee.OccupationId), "Ocupação não encontrada");

                return NotFound(ModelState);
            }

            Employee employee = input.ToModel(occupation, _passwordHash);

            await _context.Users.AddAsync(employee.User);

            await _context.Employees.AddAsync(employee);

            return Ok(new OutputEmployee(employee));
        }


        [HttpGet]
        [Route("levels")]
        [SwaggerOperation("Lists available access levels")]
        [SwaggerResponse(200, "The available access levels", typeof(IEnumerable<OutputAccessLevel>))]
        public IActionResult ListAccessLevels()
        {
            return Ok(_accessLevelHandler.ListFromEnum());
        }


        [HttpGet]
        [Route("occupations")]
        [SwaggerOperation("Lists available occupations")]
        [SwaggerResponse(200, "The available occupations", typeof(IEnumerable<Occupation>))]
        public async Task<IActionResult> ListOccupations()
        {
            return Ok(await _context.Occupations.ToListAsync());
        }


        private async Task<IActionResult> UpdateEntry(InputEmployee input)
        {
            string cpf = input.Cpf;

            var entry = await _context.Employees
                .FindAsync(cpf);

            await _context.Entry(entry)
                .Reference(row => row.User)
                .LoadAsync();

            if (entry == null)
            {
                return NotFound();
            }

            var occupation = await _context.Occupations.FindAsync(input.OccupationId);

            if (occupation == null)
            {
                return NotFound();
            }

            Employee updated = input.ToModel(occupation, _passwordHash);

            entry.User.Name = updated.User.Name;
            entry.User.SocialName = updated.User.SocialName;
            entry.User.BirthDate = updated.User.BirthDate;
            entry.User.Phone = updated.User.Phone;
            entry.User.Email = updated.User.Email;
            entry.Gender = updated.Gender;
            entry.Rg = updated.Rg;
            entry.OccupationId = updated.OccupationId;
            entry.AccessLevel = updated.AccessLevel;

            // we ignore password changes if the input 
            // did not request one.

            if (input.Password != null)
            {
                entry.User.Password = updated.User.Password;
            }

            await _context.SaveChangesAsync();

            return Ok(new OutputEmployee(entry));
        }
    }
}
