using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal BaseValue { get; set; }
        public decimal PricePerPhoto { get; set; }
        public int? ImageQuantity { get; set; }
        public int? QuantityMultiplier { get; set; }
        public int? MaxIterations { get; set; }
        public bool Available { get; set; }

    }
}
