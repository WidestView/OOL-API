using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models.DataTransfer
{
    public class InputOrder
    {
        [Required(ErrorMessage = "O pacote é obrigatório")]
        public int PackageId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        public int ImageQuantity { get; set; }

        public Order ToModel(Customer customer, Package package, decimal price)
        {
            return new Order
            {
                BuyTime = DateTime.UtcNow,
                Customer = customer,
                CustomerId = customer.UserId,
                ImageQuantity = ImageQuantity,
                Package = package,
                PackageId = package.Id,
                Price = price
            };
        }
    }
}
