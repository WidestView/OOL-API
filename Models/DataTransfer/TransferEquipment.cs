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

        public virtual Equipment ToModel()
        {
            return new Equipment
            {
                Available = Available,
                Details = null,
                DetailsId = DetailsId
            };
        }

        public class ForUpdate : InputEquipment
        {
            [Required]
            public int Id { get; set; }
        }
    }


    public class OutputEquipment
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            Details = 1 << 0,
            All = Details
        }

        public OutputEquipment(
            Equipment equipment,
            Flags flags = 0,
            OutputEquipmentDetails.Flags detailsFlags = 0
        )
        {
            Id = equipment.Id;
            Available = equipment.Available;
            DetailsId = equipment.DetailsId;

            if ((flags & Flags.Details) != 0 && equipment.Details != null)
            {
                Details = new OutputEquipmentDetails(
                    details: equipment.Details,
                    flags: detailsFlags
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
        [Range(minimum: 0, maximum: double.MaxValue)]
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

        public class ForUpdate : InputEquipmentDetails
        {
            [Required]
            public int Id { get; set; }
        }
    }


    public class OutputEquipmentDetails
    {
        [Flags]
        public enum Flags
        {
            None = 0,

            Type = 1 << 0,

            Equipments = 1 << 1,

            All = Type | Equipments
        }

        public OutputEquipmentDetails(
            EquipmentDetails details,
            Flags flags = 0,
            OutputEquipment.Flags equipmentFlags = 0
        )
        {
            Id = details.Id;
            Name = details.Name;
            Price = details.Price;
            TypeId = details.TypeId;

            if ((flags & Flags.Type) != 0 && details.Type != null)
            {
                Type = new OutputEquipmentType(details.Type);
            }

            if ((flags & Flags.Equipments) != 0 && details.Equipments != null)
            {
                Equipments = details.Equipments
                    .Select(equipment => new OutputEquipment(equipment: equipment, flags: equipmentFlags));
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
