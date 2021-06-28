﻿using OOL_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOL_API.Data
{
    public class DbInitializer
    {   
        public static void Initialize(StudioContext context)
        {
            context.Database.EnsureDeleted(); //DROP DATABASE
            context.Database.EnsureCreated();

            InitializeProducts(context);
            InitializeUsers(context);
        }

        private static void InitializeProducts(StudioContext context)
        {
            // Look for any products.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var products = new Product[]
            {
                new Product{ Name="Pacote Premium", Description="Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!", Price=20.00m },
                new Product{ Name="Pacote Extra Premium", Description="Pacote que contém tanto o ensaio fotográfico com disponibilidade digital mas também a revelação de todas fotos em um albúm de alta qualidade!", Price=25.00m }
            };

            foreach (Product p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();
        }

        private static void InitializeUsers(StudioContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var users = new User[]
            {
                new User{ Name="Adm"}
            };

            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();
        }
    }
}
