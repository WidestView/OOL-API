using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;

namespace OOL_API.Models.DataTransfer
{
    public class OutputOrder
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            Package = 1 << 0,
            Customer = 1 << 1,
            Photoshoot = 1 << 2,
            All = Package | Customer | Photoshoot
        }

        public int Id { get; set; }

        public OutputPackage Package { get; set; }

        public int ImageQuantity { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public IEnumerable<OutputPhotoShoot> Photoshoots { get; set; }

        public bool Delivered { get; set; }
    }

    public class OutputOrderHandler
    {
        private readonly StudioContext _context;

        public OutputOrderHandler(StudioContext context)
        {
            _context = context;
        }

        public OutputPackageHandler PackageHandler { get; set; }

        public Task<OutputOrder> OutputFor(Order order)
        {
            return Create(order, OutputOrder.Flags.All);
        }

        public async Task<OutputOrder> Create(Order order, OutputOrder.Flags flags)
        {
            var result = new OutputOrder
            {
                Id = order.Id,
                ImageQuantity = order.ImageQuantity,
                Price = order.Price,
                PurchaseDate = order.BuyTime,
                CustomerId = order.CustomerId,
                Photoshoots = order.PhotoShoots?.Select(p => new OutputPhotoShoot(p, true)).ToList()
            };

            if (flags.HasFlag(OutputOrder.Flags.Package))
            {
                order.Package ??= await _context.Packages.FindAsync(order.PackageId);

                result.Package = PackageHandler!.OutputFor(order.Package);
            }

            if (flags.HasFlag(OutputOrder.Flags.Customer))
            {
                order.Customer ??= await _context.Users
                    .FirstOrDefaultAsync(c => c.Cpf == order.CustomerId);

                result.CustomerName = order.Customer.SocialName ?? order.Customer.Name;
            }

            var photoshootsQuery = _context.PhotoShoots.AsQueryable().Where(p => p.OrderId == order.Id);

            if (flags.HasFlag(OutputOrder.Flags.Photoshoot))
            {
                order.PhotoShoots ??= await photoshootsQuery.Include(p => p.Images).ToListAsync();

                result.Photoshoots = order.PhotoShoots.Select(p => new OutputPhotoShoot(p, true)).ToList();
            }

            result.Delivered = photoshootsQuery
                .Any(
                    photoshoot => _context.PhotoShootImages.Any(image => image.PhotoShootId == photoshoot.Id)
                );

            return result;
        }
    }
}
