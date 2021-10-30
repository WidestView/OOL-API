using System;
using System.Text.Json.Serialization;

namespace OOL_API.Models.DataTransfer
{
#nullable disable
    public class OutputUser
    {
        public enum UserKind
        {
            Customer,
            Employee,
            SudoEmployee
        }

        [JsonIgnore]
        public UserKind KindValue { get; set; }

        public string Kind => KindValue switch
        {
            UserKind.Customer => "customer",
            UserKind.Employee => "employee",
            UserKind.SudoEmployee => "admin",
            _ => throw new ArgumentOutOfRangeException()
        };

        public string Cpf { get; set; }

        public string Name { get; set; }

        public string SocialName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }
    }

#nullable enable

    public class OutputUserHandler
    {
        public OutputUser OutputFor(Employee employee)
        {
            var kind = employee.AccessLevel switch
            {
                AccessLevel.Default => OutputUser.UserKind.Employee,
                AccessLevel.Sudo => OutputUser.UserKind.Customer,
                _ => throw new ArgumentOutOfRangeException()
            };

            return OutputFor(employee.User, kind);
        }

        public OutputUser OutputFor(User user)
        {
            return OutputFor(user, OutputUser.UserKind.Customer);
        }

        private OutputUser OutputFor(User user, OutputUser.UserKind kind)
        {
            return new OutputUser
            {
                KindValue = kind,
                Cpf = user.Cpf,
                Name = user.Name,
                SocialName = user.SocialName,
                BirthDate = user.BirthDate,
                Phone = user.Phone,
                Email = user.Email,
                Active = user.Active
            };
        }
    }
}
