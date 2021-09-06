using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OOL_API.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public string CreatorCpf { get; set; }
        public Employee Creator { get; set; }

        public int PhotoShootId { get; set; }
        public PhotoShoot PhotoShoot { get; set; }
    }
}
