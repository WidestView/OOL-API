using System;
using OOL_API.Data;
using static OOL_API.Models.DataTransfer.OutputEquipment;

namespace OOL_API.Models.DataTransfer
{
    public class OutputEquipment
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            Details = 1 << 0,
            All = Details
        }

        public int Id { get; set; }

        public bool Available { get; set; }

        public int DetailsId { get; set; }

        public OutputEquipmentDetails Details { get; set; }
    }

    public class OutputEquipmentHandler
    {
        private readonly StudioContext _context;

        private readonly OutputEquipmentDetails.Flags _detailsFlags =
            OutputEquipmentDetails.Flags.All ^ OutputEquipmentDetails.Flags.Equipments;

        private readonly Flags _outputFlags = Flags.All;


        public OutputEquipmentHandler(StudioContext context)
        {
            _context = context;
        }

        public OutputEquipmentDetailsHandler DetailsHandler { get; set; }

        public OutputEquipment OutputFor(Equipment equipment)
        {
            return Create(equipment: equipment, flags: _outputFlags);
        }

        public OutputEquipment Create(Equipment equipment, Flags flags)
        {
            var result = new OutputEquipment
            {
                DetailsId = equipment.DetailsId,
                Id = equipment.Id,
                Available = equipment.Available
            };

            if ((flags & Flags.Details) != 0)
            {
                if (equipment.Details == null)
                {
                    _context.Entry(equipment)
                        .Reference(row => row.Details)
                        .Load();
                }

                result.Details = DetailsHandler.Create(details: equipment.Details, flags: _detailsFlags);
            }

            return result;
        }
    }
}
