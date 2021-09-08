using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OOL_API.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string Hack { get; set; } // Hack String because EF CORE SUCKS!!!
    }
}
