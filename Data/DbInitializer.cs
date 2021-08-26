using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using OOL_API.Models;

namespace OOL_API.Data
{
    public class DbInitializer
    {
        private readonly bool _resetDatabase;

        public DbInitializer(IConfiguration configuration)
        {
            _resetDatabase = bool.Parse(configuration["ResetDatabaseOnBoot"] ?? "true");
        }

        public void Initialize(StudioContext context)
        {
            if (_resetDatabase) context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            var alreadyInitialized = context.Packages.Any();

            if (alreadyInitialized) return;

            AddPackages(context);

            AddPhotoShoots(context);

            AddEmployees(context);

            context.SaveChanges();
        }

        private static void AddEmployees(StudioContext context)
        {
            var occupation = new Occupation
            {
                Description = "Sleep",
                Name = "Idk"
            };

            var employees = new[]
            {
                new Employee
                {
                    AcessLevel = 1,
                    Gender = "Attack Helicopter",
                    Occupation = occupation,
                    User = new User
                    {
                        Active = true,
                        BirthDate = DateTime.Now - TimeSpan.FromDays(6570),
                        Cpf = "11111111111",
                        Email = "some@email.com",
                        Name = "bob",
                        Password = "tohash",
                        Phone = "40028922",
                        SocialName = null
                    }
                }
            };

            context.Occupations.Add(occupation);

            foreach (var employee in employees)
            {
                employee.OccupationId = employee.Occupation.Id;

                context.Users.Add(employee.User);

                employee.UserId = employee.User.Cpf;

                context.Employees.Add(employee);
            }
        }

        private static void AddPackages(StudioContext context)
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

            foreach (var p in packages) context.Packages.Add(p);
        }

        private static void AddPhotoShoots(StudioContext context)
        {
            var shoots = new[]
            {
                new PhotoShoot
                {
                    Address = "localhost avenue",
                    Duration = TimeSpan.FromHours(1),
                    OrderId = 10,
                    ResourceId = Guid.Parse("5a60a77f-e51b-4aa6-7b3c-08d94570814c")
                },

                new PhotoShoot
                {
                    Address = "127001 street",
                    Duration = TimeSpan.FromHours(1),
                    OrderId = 10
                }
            };

            foreach (var photoShoot in shoots) context.PhotoShoots.Add(photoShoot);
        }
    }
}