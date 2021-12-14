using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }

        public int ImageQuantity { get; set; }

        public decimal Price { get; set; }

        public DateTime BuyTime { get; set; }

        public string CustomerId { get; set; }

        public Customer Customer { get; set; }

        public List<PhotoShoot> PhotoShoots { get; set; }
    }
}
