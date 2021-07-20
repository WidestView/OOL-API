using System;
using System.Linq;
using OOL_API.Models;

namespace OOL_API.Data
{
    public class DbInitializer
    {
        // todo: move to configuration
        private static readonly bool ResetDatabase = true;
        
        public static void Initialize(StudioContext context)
        {
            if (ResetDatabase)
            {
                context.Database.EnsureDeleted(); //DROP DATABASE
                context.Database.EnsureCreated();
            }

            // Look for any packages.
            if (context.Packages.Any())
            {
                return;   // DB has been seeded
            }

            var packages = new[]
            {
                new Package{
                    Name = "Premium",
                    Description = "Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!",
                    BaseValue = 50.00m,
                    PricePerPhoto = 2.50m,
                    ImageQuantity = null,
                    QuantityMultiplier = 25,
                    MaxIterations = 5,
                    Image = "batata.jpg",
                    Available = true
                },
                new Package{
                    Name = "Você Modelo!",
                    Description = "Pacote com um ensaio fotográfico e a disponibilidade das fotos por acesso digital a nossa plataforma!",
                    BaseValue = 200.0m,
                    PricePerPhoto = 0.00m,
                    ImageQuantity = 200,
                    QuantityMultiplier = null,
                    MaxIterations = null,
                    Image = "batata.jpg",
                    Available = true
                }
            };

            foreach (Package p in packages)
            {
                context.Packages.Add(p);
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
