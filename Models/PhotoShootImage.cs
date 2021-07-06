using System;
using System.ComponentModel.DataAnnotations;

namespace OOL_API.Models
{
    public class PhotoShootImage
    {
        [Key]
        public Guid Id { get; set; }

        public int PhotoShootId { get; set; }

        public PhotoShoot PhotoShoot { get; set; }
        
        public bool OnPortfolio { get; set; }
    }
}
