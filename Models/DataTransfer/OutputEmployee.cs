using System;

namespace OOL_API.Models.DataTransfer
{
    public class OutputEmployee
    {
        public OutputEmployee(Employee employee)
        {
            Cpf = employee.User.Cpf;
            Name = employee.User.Name;
            SocialName = employee.User.SocialName;
            BirthDate = employee.User.BirthDate;
            Phone = employee.User.Phone;
            Email = employee.User.Email;
            AccessLevel = employee.AccessLevel;
            OccupationId = employee.OccupationId;
            Occupation = employee.Occupation;
            Gender = employee.Gender;
            RG = employee.RG;
        }

        public string Cpf { get; }

        public string Name { get; }

        public string SocialName { get; }

        public DateTime BirthDate { get; }

        public string Phone { get; }

        public string Email { get; }

        public int AccessLevel { get; }

        public int OccupationId { get; }

        public Occupation Occupation { get; }

        public string Gender { get; }

        public string RG { get; }
    }
}