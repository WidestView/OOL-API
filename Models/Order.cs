using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OOL_API.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public bool Delivered { get; set; }

        public int ImageQuantity { get; set; }

        public decimal Price { get; set; }

        public DateTime Delivering { get; set; }
    }
}
