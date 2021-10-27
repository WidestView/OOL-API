using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OOL_API.Models;
using OOL_API.Services;

namespace OOL_API.Data
{
    public class DbInitializer
    {
        private readonly IPasswordHash _hash;
        private readonly IAppSettings _settings;

        public DbInitializer(IPasswordHash hash, IAppSettings settings)
        {
            _hash = hash;
            _settings = settings;
        }

        public async Task Initialize(StudioContext context)
        {
            if (_settings.ResetDatabaseOnBoot)
            {
                await context.Database.EnsureDeletedAsync();
            }

            await context.Database.EnsureCreatedAsync();

            var alreadyInitialized = context.Packages.Any();

            if (alreadyInitialized)
            {
                return;
            }

            await CreatePackages(context);

            await CreateUsers(context);

            await CreateEmployees(context);

            await CreateCustomers(context);

            await CreateOrders(context);

            await CreatePhotoshoots(context);

            await CreateEquipments(context);

            await CreateWithdraw(context);
        }

        private async Task CreateWithdraw(StudioContext context)
        {
            var employee = await context.Employees.OrderBy(row => row.UserId).FirstAsync();

            var equipment = await context.Equipments.OrderBy(row => row.Id).FirstAsync();

            var photoshoot = await context.PhotoShoots.OrderBy(row => row.Id).FirstAsync();

            var withdraw = new EquipmentWithdraw
            {
                WithdrawDate = DateTime.UtcNow,
                ExpectedDevolutionDate = DateTime.UtcNow + TimeSpan.FromHours(5),
                EffectiveDevolutionDate = DateTime.UtcNow + TimeSpan.FromHours(3),
                Employee = employee,
                EmployeeCpf = employee.UserId,
                Equipment = equipment,
                EquipmentId = equipment.Id,
                PhotoShootId = photoshoot.Id,
                PhotoShoot = photoshoot
            };

            await context.EquipmentWithDraws.AddAsync(withdraw);

            await context.SaveChangesAsync();
        }

        private async Task CreatePackages(StudioContext context)
        {
            var packages = new[]
            {
                new Package
                {
                    Name = "Premium",
                    Description =
                        "Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!",
                    BaseValue = 50.00m,
                    PricePerPhoto = 2.50m,
                    ImageQuantity = null,
                    QuantityMultiplier = 25,
                    MaxIterations = 5,
                    Available = true
                },
                new Package
                {
                    Name = "Você Modelo!",
                    Description =
                        "Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!",
                    BaseValue = 200.0m,
                    PricePerPhoto = 0.00m,
                    ImageQuantity = 200,
                    QuantityMultiplier = null,
                    MaxIterations = null,
                    Available = true
                }
            };

            foreach (var p in packages)
            {
                await context.Packages.AddAsync(p);
            }

            await context.SaveChangesAsync();
        }

        private async Task CreateUsers(StudioContext context)
        {
            var users = new[]
            {
                new User
                {
                    Active = true,
                    BirthDate = DateTime.UtcNow - TimeSpan.FromDays(6570),
                    Cpf = _settings.DefaultUserCpf,
                    Email = _settings.DefaultUserEmail,
                    Name = "bob",
                    Password = _hash.Of(_settings.DefaultUserPassword),
                    Phone = "40028922",
                    SocialName = null
                },
                new User
                {
                    Active = false,
                    BirthDate = DateTime.UtcNow - TimeSpan.FromDays(14600),
                    Cpf = _settings.SuperUserCpf,
                    Email = _settings.SuperUserEmail,
                    Name = "super bob",
                    Password = _hash.Of(_settings.SuperUserPassword),
                    Phone = "0101010101",
                    SocialName = "root sudoer"
                }
            };

            foreach (var user in users)
            {
                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }

        private async Task CreateEmployees(StudioContext context)
        {
            var occupation = new Occupation
            {
                Description = "Sleep",
                Name = "Idk"
            };

            await context.Occupations.AddAsync(occupation);
            await context.SaveChangesAsync();

            var employees = new[]
            {
                new Employee
                {
                    AccessLevel = AccessLevel.Default,
                    Gender = "Attack Helicopter",
                    Rg = "102010102010",
                    OccupationId = occupation.Id
                },
                new Employee
                {
                    AccessLevel = AccessLevel.Sudo,
                    Gender = "IEEE 754 Standard for Floating-Point Arithmetic",
                    Rg = "102010102010",
                    OccupationId = occupation.Id
                }
            };

            foreach (var (employee, user) in employees.Zip(await context.Users.ToListAsync()))
            {
                employee.UserId = user.Cpf;

                await context.Employees.AddAsync(employee);
            }

            await context.SaveChangesAsync();
        }

        private async Task CreateCustomers(StudioContext context)
        {
            var user = await context.Users.OrderBy(row => row.Cpf).FirstAsync();

            var cart = new Cart();

            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();

            var customers = new[]
            {
                new Customer
                {
                    UserId = user.Cpf,
                    CartId = cart.Id
                }
            };

            foreach (var customer in customers)
            {
                await context.Customers.AddAsync(customer);
            }

            await context.SaveChangesAsync();
        }

        private async Task CreateOrders(StudioContext context)
        {
            var customer = await context.Customers.OrderBy(row => row.UserId).FirstAsync();
            var package = await context.Packages.OrderBy(row => row.Id).FirstAsync();

            var order = new Order
            {
                CartId = customer.CartId,
                PackageId = package.Id
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }

        private async Task CreatePhotoshoots(StudioContext context)
        {
            var order = await context.Orders.OrderBy(row => row.Id).FirstAsync();

            var employee = await context.Employees.OrderBy(row => row.UserId).FirstAsync();

            var shoots = new[]
            {
                new PhotoShoot
                {
                    Address = "localhost Avenue",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow + TimeSpan.FromHours(2),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "127001 Street",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow - TimeSpan.FromHours(1),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "0.0.0.0 City",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow + TimeSpan.FromDays(7),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "::1 Town",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow + TimeSpan.FromDays(3),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                }
            };

            foreach (var photoShoot in shoots)
            {
                await context.PhotoShoots.AddAsync(photoShoot);
            }

            await context.SaveChangesAsync();
        }

        private async Task CreateEquipments(StudioContext context)
        {
            var types = new[]
            {
                new EquipmentType
                {
                    Name = "Good Camera",
                    Description = "A high quality camera"
                },

                new EquipmentType
                {
                    Name = "Better Camera",
                    Description = "A higher quality camera"
                }
            };

            foreach (var t in types)
            {
                await context.EquipmentTypes.AddAsync(t);
            }

            await context.SaveChangesAsync();

            var type = types.OrderBy(row => row.Id).First();

            var details = new EquipmentDetails
            {
                Name = "Kodak Camera 0xIDK",
                Price = 400.00m,
                TypeId = type.Id
            };

            await context.SaveChangesAsync();

            await context.EquipmentDetails.AddAsync(details);

            var equipments = new[]
            {
                new Equipment
                {
                    Available = true,
                    Details = details,
                    DetailsId = details.Id
                }
            };

            foreach (var equipment in equipments)
            {
                await context.Equipments.AddAsync(equipment);
            }

            await context.SaveChangesAsync();
        }
    }
}
