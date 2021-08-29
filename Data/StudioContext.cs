using Microsoft.EntityFrameworkCore;
using OOL_API.Models;

namespace OOL_API.Data
{
    public class StudioContext : DbContext
    {
        public StudioContext(DbContextOptions<StudioContext> options) : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }

        public DbSet<PhotoShootImage> PhotoShootImages { get; set; }

        public DbSet<PhotoShoot> PhotoShoots { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Occupation> Occupations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Package>().ToTable("Package");
            builder.Entity<PhotoShoot>().ToTable("PhotoShoot");
            builder.Entity<User>().ToTable("User");
            builder.Entity<Occupation>().ToTable("Occupation");

            builder.Entity<PhotoShootImage>()
                .ToTable("PhotoShootImage")
                .HasOne(image => image.PhotoShoot)
                .WithMany(shot => shot.Images)
                .HasForeignKey(image => image.PhotoShootId)
                .IsRequired();

            var employeeTable = builder.Entity<Employee>()
                .ToTable("Employee");

            employeeTable.HasOne(employee => employee.Occupation)
                .WithMany()
                .HasForeignKey(employee => employee.OccupationId)
                .IsRequired();

            employeeTable.HasOne(employee => employee.User)
                .WithOne()
                .HasForeignKey<Employee>(employee => employee.UserId)
                .IsRequired();
        }
    }
}