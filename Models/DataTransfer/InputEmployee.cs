using System.ComponentModel.DataAnnotations;
using OOL_API.Services;

namespace OOL_API.Models.DataTransfer
{
    public class InputEmployee : InputUser
    {
        [Required(ErrorMessage = "O nível de acesso é obrigatório")]
        public AccessLevel AccessLevel { get; set; }

        [Required(ErrorMessage = "O genêro é obrigatório (mas talvez não deveria ser assim)")]
        [MaxLength(64, ErrorMessage = "O gênero deve ter até 64 caracteres.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "O RG é obrigatório")]
        [MaxLength(20, ErrorMessage = "O RG deve ter no máximo 20 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O RG deve ser numérico")]
        public string Rg { get; set; }

        [Required(ErrorMessage = "A ocupação é obrigatória")]
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
                    Cpf = Misc.StripCpf(Cpf),
                    Email = Email,
                    Name = Name,
                    Password = Password == null
                        ? null
                        : passwordHash.Of(Password),
                    Phone = Phone,
                    SocialName = SocialName
                },
                UserId = Cpf
            };
        }
    }
}
