using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOL_API.Models
{
    public class PhotoShoot
    {
        [Key]
        public int Id { get; set; }
        
        // Only works with key, manual generation required.
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid ResourceId { get; set; } = Guid.NewGuid();
        
        public int OrderId { get; set; }
        
        public string Address { get; set; }
        
        public DateTime Start { get; set; }
        
        public TimeSpan Duration { get; set; }
        
        public List<PhotoShootImage> Images { get; set; }
    }
}