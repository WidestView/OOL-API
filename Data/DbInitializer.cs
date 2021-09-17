﻿using System;
using System.Collections.Generic;
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

            CreatePackages(context);

            var users = CreateUsers(context);

            CreateEmployees(context, users);

            CreateCustomers(context);

            CreateOrders(context);

            CreatePhotoshoots(context);

            CreateEquipments(context);
        }

        private void CreatePackages(StudioContext context)
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
        }

        private User[] CreateUsers(StudioContext context)
        {
            var users = new[]
            {
                new User
                {
                    Active = true,
                    BirthDate = DateTime.Now - TimeSpan.FromDays(6570),
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
                    BirthDate = DateTime.Now - TimeSpan.FromDays(14600),
                    Cpf = _settings.SuperUserCpf,
                    Email = _settings.SuperUserEmail,
                    Name = "super bob",
                    Password = _hash.Of(_settings.SuperUserPassword),
                    Phone = "0101010101",
                    SocialName = "root sudoer"
                }
            };

            foreach (var user in users) context.Users.Add(user);
            context.SaveChanges();
            return users;
        }

        private void CreateEmployees(StudioContext context, IEnumerable<User> users)
        {
            var occupation = new Occupation
            {
                Description = "Sleep",
                Name = "Idk"
            };

            context.Occupations.Add(occupation);
            context.SaveChanges();

            var employees = new[]
            {
                new Employee
                {
                    AccessLevel = AccessLevel.Default,
                    Gender = "Attack Helicopter",
                    OccupationId = occupation.Id
                },
                new Employee
                {
                    AccessLevel = AccessLevel.Sudo,
                    Gender = "IEEE 754 Standard for Floating-Point Arithmetic",
                    OccupationId = occupation.Id
                }
            };

            foreach (var (employee, user) in employees.Zip(users))
            {
                employee.UserId = user.Cpf;

                context.Employees.Add(employee);
            }

            context.SaveChanges();
        }

        private void CreateCustomers(StudioContext context)
        {
            var user = context.Users.First();

            var cart = new Cart();
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
        }

        private void CreateOrders(StudioContext context)
        {
            var customer = context.Customers.First();
            var package = context.Packages.First();

            var order = new Order
            {
                CartId = customer.CartId,
                PackageId = package.Id
            };

            context.Orders.Add(order);
            context.SaveChanges();
        }

        private void CreatePhotoshoots(StudioContext context)
        {
            var order = context.Orders.First();

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
        }

        private void CreateEquipments(StudioContext context)
        {
            var type = new EquipmentType
            {
                Name = "Good Camera",
                Description = "A high quality camera"
            };

            context.EquipmentTypes.Add(type);

            context.SaveChanges();

            var details = new EquipmentDetails
            {
                Name = "Kodak Camera 0xIDK",
                Price = 400.00m,
                TypeId = type.Id
            };

            context.SaveChanges();

            context.EquipmentDetails.Add(details);

            var equipments = new[]
            {
                new Equipment
                {
                    Available = true,
                    Details = details,
                    DetailsId = details.Id
                }
            };

            foreach (var equipment in equipments) context.Equipments.Add(equipment);

            context.SaveChanges();
        }
    }
}