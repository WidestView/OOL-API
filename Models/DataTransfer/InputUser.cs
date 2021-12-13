using Microsoft.AspNetCore.Mvc;
using OOL_API.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// ReSharper disable MemberCanBeProtected.Global

namespace OOL_API.Models.DataTransfer
{
    public class InputUser : IValidatableObject
    {
        [Required]
        [MinLength(11)]
        [MaxLength(14)]
        [Remote(action: "VerifyCpf", controller: "User")]
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
        [Remote(action: "VerifyEmail", controller: "User")]
        public string Email { get; set; }

        [MaxLength(64)]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = DateTime.Now - BirthDate;

            int age = span.TotalSeconds > 0? (zeroTime + span).Year - 1 : 0;

            if (age < 12) 
            {
                yield return new ValidationResult(
                    "The User must be at least 12 years old, as described in our Terms of Service", new string[] { nameof(BirthDate) }
                );
            }
        }

        public void HashPassword(IPasswordHash passwordHash) => Password = passwordHash.Of(Password);

        public User ToModel() => new User()
        {
            Cpf = Cpf.Replace(".", "").Replace("-", ""),
            Name = Name,
            SocialName = SocialName,
            BirthDate = BirthDate,
            Phone = Phone,
            Email = Email,
            Active = true,
            Password = Password
        };
    }
}
