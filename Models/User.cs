using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class User
    {
        [Required]
        [Key]
        public string Cpf { get; set; }

        [Required]
        public string Name { get; set; }

        public string SocialName { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public bool Active { get; set; }

        [Required]
        public string Password { get; set; }
    }
}