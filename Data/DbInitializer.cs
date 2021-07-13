using System;
using System.Linq;
using OOL_API.Models;

namespace OOL_API.Data
{
    public class DbInitializer
    {
        // todo: move to configuration
        private static readonly bool ResetDatabase = false;
        
        public static void Initialize(StudioContext context)
        {
            if (ResetDatabase)
            {
                context.Database.EnsureDeleted(); //DROP DATABASE
                context.Database.EnsureCreated();
            }

            // Look for any products.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var products = new[]
            {
                new Product{ Name="Pacote Premium", Description="Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!", Price=20.00m },
                new Product{ Name="Pacote Extra Premium", Description="Pacote que contém tanto o ensaio fotográfico com disponibilidade digital mas também a revelação de todas fotos em um albúm de alta qualidade!", Price=25.00m }
            };

            foreach (Product p in products)
            {
                context.Products.Add(p);
            }

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
                    OrderId = 10,
                },
            };

            foreach (var photoShoot in shoots)
            {
                context.PhotoShoots.Add(photoShoot);
            }
            
            context.SaveChanges();
        }
    }
}
