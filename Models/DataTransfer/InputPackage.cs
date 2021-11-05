// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models.DataTransfer
{
    public class InputPackage : IValidatableObject
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseValue { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerPhoto { get; set; }

        [Range(1, int.MaxValue)]
        public int? ImageQuantity { get; set; }

        [Range(1, int.MaxValue)]
        public int? QuantityMultiplier { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxIterations { get; set; }

        public bool Available { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImageQuantity != null && (QuantityMultiplier != null || MaxIterations != null))
            {
                yield return new ValidationResult(
                    "If the image quantity is set, the quantity multiplier and max iterations must not be set"
                );
            }

            if ((QuantityMultiplier == null ? 1 : 0) + (MaxIterations == null ? 1 : 0) == 1)
            {
                yield return new ValidationResult(
                    "The quantity multiplier and max iterations must be set together"
                );
            }

            if (ImageQuantity == null && QuantityMultiplier == null && MaxIterations == null)
            {
                yield return new ValidationResult(
                    "The package quantity must be set, " +
                    "either through image quantity or quantity multiplier + max iterations");
            }
        }

        public Package ToModel()
        {
            return new Package
            {
                Name = Name,
                Description = Description,
                BaseValue = BaseValue,
                PricePerPhoto = PricePerPhoto,
                ImageQuantity = ImageQuantity,
                QuantityMultiplier = QuantityMultiplier,
                MaxIterations = MaxIterations,
                Available = Available
            };
        }
    }
}
