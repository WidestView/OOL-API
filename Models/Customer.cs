using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Customer
    {
        [Key]
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
