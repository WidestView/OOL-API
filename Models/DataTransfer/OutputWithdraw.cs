using System;
using System.Threading;
using System.Threading.Tasks;
using OOL_API.Data;
using static OOL_API.Models.DataTransfer.OutputWithdraw;

namespace OOL_API.Models.DataTransfer
{
    public class OutputWithdraw
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            PhotoShoot = 1 << 0,
            Employee = 1 << 1,
            Equipment = 1 << 2,
            All = PhotoShoot | Employee | Equipment
        }

        public int Id { get; set; }

        public DateTime WithdrawDate { get; set; }

        public DateTime PredictedDevolutionDate { get; set; }
        public DateTime? EffectiveDevolutionDate { get; set; }

        public OutputPhotoShoot PhotoShoot { get; set; }

        public OutputEmployee Employee { get; set; }

        public OutputEquipment Equipment { get; set; }
    }

    public class OutputWithdrawHandler
    {
        private readonly StudioContext _context;

        private readonly OutputEquipment.Flags _equipmentFlags = OutputEquipment.Flags.Details;
        private readonly Flags _outputFlags = Flags.All;

        public OutputWithdrawHandler(StudioContext context)
        {
            _context = context;
        }

        public OutputEquipmentHandler EquipmentHandler { get; set; }

        public OutputEquipmentHandler Bind(OutputEquipmentHandler handler)
        {
            EquipmentHandler = handler;
            return handler;
        }

        public Task<OutputWithdraw> OutputFor(EquipmentWithdraw withdraw, CancellationToken token)
        {
            return Create(withdraw, _outputFlags, token);
        }

        public async Task<OutputWithdraw> Create(EquipmentWithdraw withdraw, Flags flags, CancellationToken token)
        {
            var result = new OutputWithdraw
            {
                Id = withdraw.Id,
                WithdrawDate = withdraw.WithdrawDate,
                PredictedDevolutionDate = withdraw.ExpectedDevolutionDate,
                EffectiveDevolutionDate = withdraw.EffectiveDevolutionDate
            };

            if ((flags & Flags.Employee) != 0)
            {
                if (withdraw.Employee == null)
                {
                    await _context
                        .Entry(withdraw)
                        .Reference(row => row.Employee)
                        .LoadAsync(token);
                }

                if (withdraw.Employee!.User == null)
                {
                    await _context
                        .Entry(withdraw.Employee)
                        .Reference(row => row.User)
                        .LoadAsync(token);
                }

                result.Employee = new OutputEmployee(withdraw.Employee);
            }

            if ((flags & Flags.Equipment) != 0)
            {
                EquipmentHandler = EquipmentHandler
                                   ?? throw new ArgumentNullException(nameof(EquipmentHandler));

                if (withdraw.Equipment == null)
                {
                    await _context.Entry(withdraw)
                        .Reference(row => row.Employee)
                        .LoadAsync(token);
                }

                result.Equipment = await EquipmentHandler.Create(withdraw.Equipment, _equipmentFlags, token);
            }


            if ((flags & Flags.PhotoShoot) != 0)
            {
                if (withdraw.PhotoShoot == null)
                {
                    await _context.Entry(withdraw)
                        .Reference(row => row.Employee)
                        .LoadAsync(token);
                }

                result.PhotoShoot = new OutputPhotoShoot(withdraw.PhotoShoot, withReferences: false);
            }

            return result;
        }
    }
}
