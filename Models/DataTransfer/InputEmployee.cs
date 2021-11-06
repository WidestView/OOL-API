using System.ComponentModel.DataAnnotations;
using OOL_API.Services;

namespace OOL_API.Models.DataTransfer
{
    public class InputEmployee : InputUser
    {
        [Required]
        public AccessLevel AccessLevel { get; set; }

        [Required]
        [MaxLength(64, ErrorMessage = "Go easy buddy")]
        public string Gender { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^\d+$")]
        public string Rg { get; set; }

        [Required]
        public int OccupationId { get; set; }

        public Employee ToModel(Occupation occupation, IPasswordHash passwordHash)
        {
            return new Employee
            {
                AccessLevel = AccessLevel,
                Gender = Gender,
                Occupation = occupation,
                OccupationId = occupation.Id,
                ParticipatingPhotoShoots = null,
                Rg = Rg,
                User = new User
                {
                    Active = true,
                    BirthDate = BirthDate,
                    Cpf = Cpf,
                    Email = Email,
                    Name = Name,
                    Password = passwordHash.Of(Password),
                    Phone = Phone,
                    SocialName = SocialName
                },
                UserId = Cpf
            };
        }
    }
}
