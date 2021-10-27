using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public Task<OutputEquipmentDetails> OutputFor(EquipmentDetails details, CancellationToken token = default)
        {
            return Create(details, _outputFlags, token);
        }

        public async Task<OutputEquipmentDetails> Create(EquipmentDetails details, Flags flags,
            CancellationToken token = default)
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
                    await _context
                        .Entry(details)
                        .Reference(row => row.Type)
                        .LoadAsync(token);
                }

                result.Type = new OutputEquipmentType(details.Type);
            }

            if ((flags & Flags.Equipments) != 0)
            {
                EquipmentHandler = EquipmentHandler
                                   ?? throw new ArgumentNullException(nameof(EquipmentHandler));

                if (details.Equipments == null)
                {
                    await _context
                        .Entry(details)
                        .Collection(row => row.Equipments)
                        .LoadAsync(token);
                }

                var equipments = details.Equipments!
                    .Select(async equipment =>
                        await EquipmentHandler.Create(equipment, _equipmentFlags, token)
                    );

                result.Equipments = await Task.WhenAll(equipments);
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
