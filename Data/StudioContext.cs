using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using OOL_API.Models;

namespace OOL_API.Data
{
    public class StudioContext : DbContext
    {
        public StudioContext(DbContextOptions<StudioContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        
        public DbSet<PhotoShootImage> PhotoShootImages { get; set; }
        
        public DbSet<PhotoShoot> PhotoShoots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<PhotoShoot>().ToTable("PhotoShoot");
            
            modelBuilder.Entity<PhotoShootImage>()
                .ToTable("PhotoShootImage")
                .HasOne(image => image.PhotoShoot)
                .WithMany(shot => shot.Images)
                .HasForeignKey(image => image.PhotoShootId)
                .IsRequired();
        }
    }
}
