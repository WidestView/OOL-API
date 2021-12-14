using System;
using System.Linq;
using System.Threading.Tasks;
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
            All = Package | Customer
        }

        public int Id { get; set; }

        public OutputPackage Package { get; set; }

        public int ImageQuantity { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }

        // todo: OutputCustomer here 0-0
        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

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
                CustomerId = order.CustomerId
            };

            if (flags.HasFlag(OutputOrder.Flags.Package))
            {
                order.Package ??= await _context.Packages.FindAsync(order.PackageId);

                result.Package = PackageHandler!.OutputFor(order.Package);
            }

            if (flags.HasFlag(OutputOrder.Flags.Customer))
            {
                order.Customer ??= await _context.Customers.FindAsync();

                result.CustomerName = order.Customer.User.SocialName ?? order.Customer.User.Name;
            }

            result.Delivered = _context.PhotoShoots.Where(p => p.OrderId == order.Id)
                .Any(
                    photoshoot => _context.PhotoShootImages.Any(image => image.PhotoShootId == photoshoot.Id)
                );

            return result;
        }
    }
}
