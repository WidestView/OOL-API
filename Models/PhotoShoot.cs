using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OOL_API.Models
{
    public class PhotoShoot
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        // ⇑ Only works with key attributes, manual generation required.
        
        public Guid ResourceId { get; set; } = Guid.NewGuid();
        
        public int OrderId { get; set; }
        
        public string Address { get; set; }
        
        public DateTime Start { get; set; }
        
        public TimeSpan Duration { get; set; }
        
        public List<PhotoShootImage> PhotoShootImages { get; set; }
    }
}