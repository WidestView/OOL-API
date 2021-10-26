using System;
using System.Collections.Generic;
using System.Linq;
using OOL_API.Data;
using static OOL_API.Models.DataTransfer.OutputEquipmentDetails;

namespace OOL_API.Models.DataTransfer
{
    public class OutputEquipmentDetailsHandler
    {
        private readonly StudioContext _context;

        private readonly OutputEquipment.Flags _equipmentFlags =
            OutputEquipment.Flags.All ^ OutputEquipment.Flags.Details;

        private readonly Flags _outputFlags = Flags.All;

        public OutputEquipmentDetailsHandler(StudioContext context)
        {
            _context = context;
        }

        public OutputEquipmentHandler EquipmentHandler { get; set; }

        public OutputEquipmentDetails OutputFor(EquipmentDetails details)
        {
            return Create(details: details, flags: _outputFlags);
        }

        public OutputEquipmentDetails Create(EquipmentDetails details, Flags flags)
        {
            var result = new OutputEquipmentDetails
            {
                Id = details.Id,
                Name = details.Name,
                Price = details.Price,
                TypeId = details.TypeId
            };

            if ((flags & Flags.Type) != 0)
            {
                if (details.Type == null)
                {
                    _context
                        .Entry(details)
                        .Reference(row => row.Type)
                        .Load();
                }

                result.Type = new OutputEquipmentType(details.Type);
            }

            if ((flags & Flags.Equipments) != 0)
            {
                if (details.Equipments == null)
                {
                    _context
                        .Entry(details)
                        .Collection(row => row.Equipments)
                        .Load();
                }

                result.Equipments = details.Equipments!
                    .Select(equipment => EquipmentHandler.Create(equipment: equipment, flags: _equipmentFlags));
            }

            return result;
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

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int TypeId { get; set; }

        public OutputEquipmentType Type { get; set; }


        public IEnumerable<OutputEquipment> Equipments { get; set; }
    }
}
