using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

            var creator = context.Database.GetService<IRelationalDatabaseCreator>();

            // We first check if the database exists before initializing because
            // it can crash if we don't.

            if (!await creator.ExistsAsync())
            {
                await context.Database.EnsureCreatedAsync();

                await CreatePackages(context);

                await CreateUsers(context);

                await CreateEmployees(context);

                await CreateCustomers(context);

                await CreateOrders(context);

                await CreatePhotoshoots(context);

                await CreateEquipments(context);

                await CreateWithdraw(context);
            }
        }

        private async Task CreateWithdraw(StudioContext context)
        {
            var employee = await context.Employees.OrderBy(row => row.UserId).FirstAsync();

            var equipment = await context.Equipments.OrderBy(row => row.Id).FirstAsync();
            var equipment2 = await context.Equipments.OrderBy(row => row.Id).Skip(1).FirstAsync();

            var photoshoot = await context.PhotoShoots.OrderBy(row => row.Id).FirstAsync();
            var photoshoot2 = await context.PhotoShoots.OrderBy(row => row.Id).Skip(1).FirstAsync();

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

            var date = DateTime.UtcNow + TimeSpan.FromDays(2);

            var withdraw2 = new EquipmentWithdraw
            {
                WithdrawDate = date,
                ExpectedDevolutionDate = date + TimeSpan.FromHours(5),
                EffectiveDevolutionDate = date + TimeSpan.FromHours(3),
                Employee = employee,
                EmployeeCpf = employee.UserId,
                Equipment = equipment2,
                EquipmentId = equipment2.Id,
                PhotoShootId = photoshoot2.Id,
                PhotoShoot = photoshoot2
            };

            await context.EquipmentWithdraws.AddAsync(withdraw);
            await context.EquipmentWithdraws.AddAsync(withdraw2);

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
                        "Pacote com um ensaio fotográfico de grande flexibilidade",
                    BaseValue = 70.00m,
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
                        "Pacote onde você é a estrela da vez",
                    BaseValue = 100.0m,
                    PricePerPhoto = 5.00m,
                    ImageQuantity = 15,
                    QuantityMultiplier = null,
                    MaxIterations = null,
                    Available = true
                },
                new Package
                {
                    Name = "Casamento Fiel",
                    Description =
                        "Todas as fotos para seu casamento",
                    BaseValue = 150.0m,
                    PricePerPhoto = 10.00m,
                    ImageQuantity = null,
                    QuantityMultiplier = 10,
                    MaxIterations = 5,
                    Available = false
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
                    Name = "Giovanni Sunner",
                    Password = _hash.Of(_settings.DefaultUserPassword),
                    Phone = "11940028922",
                    SocialName = null
                },
                new User
                {
                    Active = true,
                    BirthDate = DateTime.UtcNow - TimeSpan.FromDays(14600),
                    Cpf = _settings.SuperUserCpf,
                    Email = _settings.SuperUserEmail,
                    Name = "Silva Gunther",
                    Password = _hash.Of(_settings.SuperUserPassword),
                    Phone = "11910101010",
                    SocialName = "Gilva Sunther"
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
                Description = "Um fotógrafo profissional no mercado",
                Name = "Fotógrafo Regular"
            };

            await context.Occupations.AddAsync(occupation);
            await context.SaveChangesAsync();

            var employees = new[]
            {
                new Employee
                {
                    AccessLevel = AccessLevel.Default,
                    Gender = "Masculino",
                    Rg = "102010102010",
                    OccupationId = occupation.Id
                },
                new Employee
                {
                    AccessLevel = AccessLevel.Sudo,
                    Gender = "Masculino",
                    Rg = "332330303010",
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
                    UserId = user.Cpf
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
            var package2 = await context.Packages.OrderBy(row => row.Id).Skip(1).FirstAsync();

            var order = new Order
            {
                PackageId = package.Id,
                CustomerId = customer.UserId,
                BuyTime = DateTime.UtcNow,
                ImageQuantity = 10,
                Price = 50
            };

            var order2 = new Order
            {
                PackageId = package2.Id,
                CustomerId = customer.UserId,
                BuyTime = DateTime.UtcNow,
                ImageQuantity = 10,
                Price = 50
            };

            await context.Orders.AddAsync(order);
            await context.Orders.AddAsync(order2);
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
                    Address = "Avenida Localhost",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow + TimeSpan.FromHours(2),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "Rua 127.0.0.1",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow - TimeSpan.FromHours(1),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "Condomínio 0.0.0.0",
                    Duration = TimeSpan.FromHours(1),
                    Start = DateTime.UtcNow + TimeSpan.FromDays(7),
                    OrderId = order.Id,
                    Employees = new List<Employee> {employee}
                },

                new PhotoShoot
                {
                    Address = "Vila ::1",
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
                    Name = "Camera boa",
                    Description = "Uma câmera de alta qualidade"
                },

                new EquipmentType
                {
                    Name = "Armazenamento",
                    Description = "Uma item de armazenamento"
                }
            };

            foreach (var t in types)
            {
                await context.EquipmentTypes.AddAsync(t);
            }

            await context.SaveChangesAsync();

            var camera = types.OrderBy(row => row.Id).First();
            var storage = types.OrderBy(row => row.Id).Skip(1).First();

            var details = new EquipmentDetails
            {
                Name = "Camera Kodak 0xIDK",
                Price = 400.00m,
                TypeId = camera.Id
            };

            var details2 = new EquipmentDetails
            {
                Name = "HDD 1TB",
                Price = 280.00m,
                TypeId = storage.Id
            };


            await context.EquipmentDetails.AddAsync(details);
            await context.EquipmentDetails.AddAsync(details2);

            await context.SaveChangesAsync();

            var equipments = new[]
            {
                new Equipment
                {
                    Available = true,
                    Details = details,
                    DetailsId = details.Id
                },

                new Equipment
                {
                    Available = false,
                    Details = details2,
                    DetailsId = details2.Id
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
