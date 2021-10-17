using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OOL_API.Models.DataTransfer
{
    public class InputEquipment
    {
        [Required]
        public bool Available { get; set; }

        [Required]
        public int DetailsId { get; set; }

        public Equipment ToModel()
        {
            return new Equipment
            {
                Available = Available,
                Details = null,
                DetailsId = DetailsId
            };
        }
    }

    public class OutputEquipment
    {
        public OutputEquipment(Equipment equipment, bool withReferences)
        {
            Id = equipment.Id;
            Available = equipment.Available;
            DetailsId = equipment.DetailsId;

            if (withReferences && equipment.Details != null)
            {
                Details = new OutputEquipmentDetails(
                    equipment.Details,
                    false
                );
            }
        }

        public int Id { get; }

        public bool Available { get; }

        public int DetailsId { get; }

        public OutputEquipmentDetails Details { get; set; }
    }

    public class InputEquipmentDetails
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
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

    public class OutputEquipmentDetails
    {
        public OutputEquipmentDetails(EquipmentDetails details, bool withReferences)
        {
            Id = details.Id;
            Name = details.Name;
            Price = details.Price;
            TypeId = details.TypeId;

            if (withReferences)
            {
                if (details.Type != null) Type = new OutputEquipmentType(details.Type);

                if (details.Equipments != null)
                    Equipments = details.Equipments
                        .Select(equipment => new OutputEquipment(equipment, false));
            }
        }

        public int Id { get; }

        public string Name { get; }

        public decimal Price { get; }

        public int TypeId { get; }

        public OutputEquipmentType Type { get; set; }

        public IEnumerable<OutputEquipment> Equipments { get; set; }
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
