using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Services;

// ReSharper disable MemberCanBeProtected.Global

namespace OOL_API.Models.DataTransfer
{
    public class InputUser : IValidatableObject
    {
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [MinLength(11, ErrorMessage = "O CPF precisa de ao menos 11 caracteres")]
        [MaxLength(14, ErrorMessage = "O CPF precisa de no máximo 14 caracteres")]
        [Remote("VerifyCpf", "User", ErrorMessage = "O CPF já está em uso.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(255, ErrorMessage = "O limite é de 255 caracteres")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "O limite é de 255 caracteres")]
        public string SocialName { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "A o telefone é obrigatório")]
        [MaxLength(15, ErrorMessage = "O telefone deve ter ao máximo 15 caracteres")]
        [MinLength(10, ErrorMessage = "O telefone deve ter no mínimo 10 caracteres")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress]
        [MaxLength(255, ErrorMessage = "O email deve ter ao máximo 255 caracteres")]
        [Remote("VerifyEmail", "User", ErrorMessage = "Email já em uso.")]
        public string Email { get; set; }

        [MaxLength(64, ErrorMessage = "A senha deve ter ao máximo 64 caracteres")]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var zeroTime = new DateTime(1, 1, 1);
            var span = DateTime.Now - BirthDate;

            var age = span.TotalSeconds > 0 ? (zeroTime + span).Year - 1 : 0;

            if (age < 12)
            {
                yield return new ValidationResult(
                    "O usuário deve ter pelo menos 12 anos :/",
                    new[] {nameof(BirthDate)}
                );
            }
        }

        public void HashPassword(IPasswordHash passwordHash)
        {
            Password = passwordHash.Of(Password);
        }

        public User ToModel()
        {
            return new User
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
}
