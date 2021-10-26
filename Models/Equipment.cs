using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class EquipmentBorrowing
    {
        [Key]
        public int Id { get; set; }

        public DateTime Takeout { get; set; }

        public DateTime PredictedDevolution { get; set; }

        public DateTime DefinitiveDevolution { get; set; }

        public int PhotoShootId { get; set; }
        public PhotoShoot PhotoShoot { get; set; }

        public string UserCpf { get; set; }
        public User User { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
    }

    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        public bool Available { get; set; }

        public int DetailsId { get; set; }

        public EquipmentDetails Details { get; set; }

        public bool IsArchived { get; set; } = false;
    }

    public class EquipmentDetails
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int TypeId { get; set; }

        [Required]
        public EquipmentType Type { get; set; }

        public int Quantity { get; set; }

        public IEnumerable<Equipment> Equipments { get; set; }

        public bool IsArchived { get; set; }
    }

    public class EquipmentType
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsArchived { get; set; } = false;
    }
}
