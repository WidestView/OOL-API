using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable MemberCanBeProtected.Global

namespace OOL_API.Models.DataTransfer
{
    public class InputUser
    {
        [Required]
        [MaxLength(11)]
        [RegularExpression(@"^\d+$")]
        public string Cpf { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string SocialName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(64)]
        public string Password { get; set; }
    }
}
