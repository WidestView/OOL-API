using System;
using System.Linq;
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

        public void Initialize(StudioContext context)
        {
            if (_settings.ResetDatabaseOnBoot) context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            var alreadyInitialized = context.Packages.Any();

            if (alreadyInitialized) return;

            Package[] packages = CreatePackages(context);

            User[] users = CreateUsers(context);

            Employee[] employees = CreateEmployees(context, users[0]);

            Customer[] customers = CreateCustomers(context, users[0]);

            Order[] orders = CreateOrders(context, customers[0], packages[0]);

            PhotoShoot[] photoShoots = CreatePhotoshoots(context, orders[0]);
        }

        private static Package[] CreatePackages(StudioContext context)
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
            context.SaveChanges();
            return packages;
        }

        private User[] CreateUsers(StudioContext context)
        {
            var users = new[]{
                new User
                {
                    Active = true,
                    BirthDate = DateTime.Now - TimeSpan.FromDays(6570),
                    Cpf = _settings.DefaultUserLogin,
                    Email = "some@email.com",
                    Name = "bob",
                    Password = _hash.Of(_settings.DefaultUserPassword),
                    Phone = "40028922",
                    SocialName = null
                }
            };

            foreach (var user in users) context.Users.Add(user);
            context.SaveChanges();
            return users;
        }

        private Employee[] CreateEmployees(StudioContext context, User user)
        {
            var occupation = new Occupation
            {
                Description = "Sleep",
                Name = "Idk"
            };

            context.Occupations.Add(occupation);
            context.SaveChanges();

            var employees = new[]{
                new Employee
                {
                    AcessLevel = 1,
                    Gender = "Attack Helicopter",
                    OccupationId = occupation.Id,
                    UserId = user.Cpf
                }
            };

            foreach (var employee in employees) context.Employees.Add(employee);
            context.SaveChanges();
            return employees;
        }

        private Customer[] CreateCustomers(StudioContext context, User user)
        {
            Cart cart = new Cart();
            context.Carts.Add(cart); //DEFAULT VALUES EXCEPTION, MUST FIX
            context.SaveChanges();

            var customers = new[]
            {
                new Customer
                {
                    UserId = user.Cpf,
                    CartId = cart.Id
                }
            };

            foreach (var customer in customers) context.Customers.Add(customer);
            context.SaveChanges();
            return customers;
        }

        private Order[] CreateOrders(StudioContext context, Customer customer, Package package)
        {
            var order = new Order
            {
                CartId = customer.CartId,
                PackageId = package.Id
            };

            context.Orders.Add(order);
            context.SaveChanges();
            return new[] { order };
        }

        private PhotoShoot[] CreatePhotoshoots(StudioContext context, Order order)
        {
            var shoots = new[]
            {
                new PhotoShoot
                {
                    Address = "localhost avenue",
                    Duration = TimeSpan.FromHours(1),
                    OrderId = order.Id,
                    ResourceId = Guid.Parse("5a60a77f-e51b-4aa6-7b3c-08d94570814c")
                },

                new PhotoShoot
                {
                    Address = "127001 street",
                    Duration = TimeSpan.FromHours(1),
                    OrderId = order.Id
                }
            };

            foreach (var photoShoot in shoots) context.PhotoShoots.Add(photoShoot);
            context.SaveChanges();
            return shoots;
        }
    }
}