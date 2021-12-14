using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models.DataTransfer
{
    public class InputWithdraw
    {
        public DateTime ExpectedDevolutionDate { get; set; }

        public DateTime? EffectiveDevolutionDate { get; set; }

        public Guid PhotoShootId { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string EmployeeCpf { get; set; }

        public int EquipmentId { get; set; }

        public EquipmentWithdraw ToModel(Employee employee, Equipment equipment, PhotoShoot photoShoot)
        {
            return new EquipmentWithdraw
            {
                WithdrawDate = DateTime.UtcNow,
                ExpectedDevolutionDate = ExpectedDevolutionDate,
                EffectiveDevolutionDate = EffectiveDevolutionDate,
                Employee = employee,
                EmployeeCpf = employee.UserId,
                Equipment = equipment,
                EquipmentId = equipment.Id,
                PhotoShoot = photoShoot,
                PhotoShootId = photoShoot.Id
            };
        }

        public EquipmentWithdraw ToModel((Employee, Equipment, PhotoShoot) tuple)
        {
            var (employee, equipment, photoShoot) = tuple;

            return ToModel(employee, equipment, photoShoot);
        }
    }
}
