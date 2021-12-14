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

        public DbSet<PhotoShoot> PhotoShoots { get; set; }

        public DbSet<PhotoShootImage> PhotoShootImages { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Occupation> Occupations { get; set; }

        public DbSet<ImportantAction> ImportantActions { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<EquipmentWithdraw> EquipmentWithdraws { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EquipmentDetails> EquipmentDetails { get; set; }

        public DbSet<EquipmentType> EquipmentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            NameTables(builder);

            RelateTables(builder);
        }

        private void NameTables(ModelBuilder builder)
        {
            builder.Entity<Package>().ToTable("Package");

            builder.Entity<PhotoShoot>().ToTable("PhotoShoot");

            builder.Entity<PhotoShootImage>().ToTable("PhotoShootImage");

            builder.Entity<User>().ToTable("User");

            builder.Entity<Employee>().ToTable("Employee");

            builder.Entity<Occupation>().ToTable("Occupation");

            builder.Entity<ImportantAction>().ToTable("ImportantAction");

            builder.Entity<Report>().ToTable("Report");

            builder.Entity<Customer>().ToTable("Customer");

            builder.Entity<Cart>().ToTable("Cart");

            builder.Entity<Order>().ToTable("Order");

            builder.Entity<EquipmentWithdraw>().ToTable("EquipmentBorrowing");

            builder.Entity<Equipment>().ToTable("Equipment");

            builder.Entity<EquipmentDetails>().ToTable("EquipmentDetails");

            builder.Entity<EquipmentType>().ToTable("EquipmentType");
        }

        private void RelateTables(ModelBuilder builder)
        {
            builder.Entity<PhotoShootImage>()
                .HasOne(image => image.PhotoShoot)
                .WithMany(shot => shot.Images)
                .HasForeignKey(image => image.PhotoShootId)
                .IsRequired();

            builder.Entity<PhotoShoot>()
                .HasOne(ps => ps.Order)
                .WithMany()
                .HasForeignKey(ps => ps.OrderId)
                .IsRequired();

            builder.Entity<Order>()
                .HasOne(order => order.Package)
                .WithMany()
                .HasForeignKey(order => order.PackageId)
                .IsRequired();

            builder.Entity<Customer>()
                .HasOne(customer => customer.User)
                .WithOne()
                .HasForeignKey<Customer>(customer => customer.UserId)
                .IsRequired();

            builder.Entity<Employee>()
                .HasOne(employee => employee.User)
                .WithOne()
                .HasForeignKey<Employee>(employee => employee.UserId)
                .IsRequired();

            builder.Entity<Employee>()
                .HasOne(employee => employee.Occupation)
                .WithMany()
                .HasForeignKey(employee => employee.OccupationId)
                .IsRequired();

            builder.Entity<ImportantAction>()
                .HasOne(ia => ia.Creator)
                .WithMany()
                .HasForeignKey(ia => ia.CreatorId)
                .IsRequired();

            builder.Entity<ImportantAction>()
                .HasOne(ia => ia.Affected)
                .WithMany()
                .HasForeignKey(ia => ia.AffectedId);

            builder.Entity<Report>()
                .HasOne(report => report.Creator)
                .WithMany()
                .HasForeignKey(report => report.CreatorId)
                .IsRequired();
        }
    }
}
