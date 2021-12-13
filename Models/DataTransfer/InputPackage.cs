// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models.DataTransfer
{
    public class InputPackage : IValidatableObject
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(255, ErrorMessage = "O limite é de 255 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(255, ErrorMessage = "O limite é de 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O valor base é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
        public decimal BaseValue { get; set; }

        [Required(ErrorMessage = "O preço por foto é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
        public decimal PricePerPhoto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade de imagens deve ser positiva")]
        public int? ImageQuantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O multiplicador de quantidades deve ser positivo")]
        public int? QuantityMultiplier { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "As iterações máximas devem ser positivas")]
        public int? MaxIterations { get; set; }

        public bool Available { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImageQuantity != null && (QuantityMultiplier != null || MaxIterations != null))
            {
                yield return new ValidationResult(
                    "Se a quantidade de imagens estiver definida, o multiplicador e iterações máximas não podem."
                );
            }

            if ((QuantityMultiplier == null ? 1 : 0) + (MaxIterations == null ? 1 : 0) == 1)
            {
                yield return new ValidationResult(
                    "O multiplicador e iterações máximas devem estar definidos juntos"
                );
            }

            if (ImageQuantity == null && QuantityMultiplier == null && MaxIterations == null)
            {
                yield return new ValidationResult(
                    "A quantidade do pacote deve ser definida, ou por meio da" +
                    "quantidade de imagens, ou pelo multiplicador de quantidades com as iterações máximas."
                );
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
