using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Customer
    {
        [Key]
        public string UserId { get; set; }

        public User User { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; }

    }
}
