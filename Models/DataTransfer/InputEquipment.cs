using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OOL_API.Models.DataTransfer
{
    public class InputEquipment
    {
        [Required(ErrorMessage = "A disponibilidade é obrigatória")]
        public bool Available { get; set; }

        [Required(ErrorMessage = "Os detalhes são obrigatórios")]
        public int DetailsId { get; set; }

        public virtual Equipment ToModel()
        {
            return new Equipment
            {
                Available = Available,
                Details = null,
                DetailsId = DetailsId
            };
        }
    }


    public class InputEquipmentDetails
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(255, ErrorMessage = "O limite é de 255 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "O tipo de equipamento é obrigatório")]
        public int TypeId { get; set; }

        public EquipmentDetails ToModel()
        {
            return new EquipmentDetails
            {
                Name = Name,
                Price = Price,
                Equipments = null,
                TypeId = TypeId,
                Type = null
            };
        }
    }


    public class OutputEquipmentType
    {
        public OutputEquipmentType(EquipmentType type)
        {
            Id = type.Id;
            Name = type.Name;
            Description = type.Description;
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }
    }

    public class InputEquipmentType
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public EquipmentType ToModel()
        {
            return new EquipmentType
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
