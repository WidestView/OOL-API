using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class Image
    {
        public Guid ID { get; set; }
        public string Path {get; set;}
        public int OwnerID { get; set; }
    }
}
