using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Employee
    {
        [Key]
        public string UserId { get; set; }
        public User User { get; set; }

        public int AcessLevel { get; set; }

        public string Gender { get; set; }

        public string RG { get; set; }

        public int OccupationId { get; set; }
        public Occupation Occupation { get; set; }
    }

    public enum Tags { HIRING, FIRING }

    public class ImportantAction 
    {
        [Key]
        public int Id { get; set; }

        public Tags Tag { get; set; }

        public string Description { get; set; }

        public string CreatorId { get; set; }
        public Employee Creator { get; set; }

        public string AffectedId { get; set; }
        public Employee Affected { get; set; }
    }

    public class Occupation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}