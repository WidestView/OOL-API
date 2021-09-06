using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Employee
    {
        [Key]
        public string UserId { get; set; }

        public User User { get; set; }

        public int AccessLevel { get; set; }

        public int OccupationId { get; set; }

        public Occupation Occupation { get; set; }

        public string Gender { get; set; }

        public string RG { get; set; }
    }
}