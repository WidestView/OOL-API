using System;
using System.Threading;
using System.Threading.Tasks;
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

        public OutputEquipmentHandler Bind(OutputEquipmentDetailsHandler handler)
        {
            DetailsHandler = handler;
            handler.EquipmentHandler = this;

            return this;
        }

        public async Task<OutputEquipment> OutputFor(Equipment equipment, CancellationToken token = default)
        {
            return await Create(equipment, _outputFlags, token);
        }

        public async Task<OutputEquipment> Create(Equipment equipment, Flags flags, CancellationToken token = default)
        {
            var result = new OutputEquipment
            {
                DetailsId = equipment.DetailsId,
                Id = equipment.Id,
                Available = equipment.Available
            };

            if ((flags & Flags.Details) != 0)
            {
                DetailsHandler = DetailsHandler
                                 ?? throw new ArgumentNullException(nameof(DetailsHandler));

                if (equipment.Details == null)
                {
                    await _context.Entry(equipment)
                        .Reference(row => row.Details)
                        .LoadAsync(token);
                }

                result.Details = await DetailsHandler.Create(equipment.Details, _detailsFlags, token);
            }

            return result;
        }
    }
}
