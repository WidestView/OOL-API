using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OOL_API.Models.DataTransfer
{
    public class InputPhotoShoot
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public uint DurationMinutes { get; set; }

        public PhotoShoot ToPhotoShoot()
        {
            return new PhotoShoot
            {
                Address = Address,
                Duration = TimeSpan.FromMinutes(DurationMinutes),
                OrderId = OrderId,
                Start = Start
            };
        }
    }

    public class OutputPhotoShoot
    {
        public OutputPhotoShoot(PhotoShoot photoShoot, bool withReferences)
        {
            Id = photoShoot.ResourceId;
            OrderId = photoShoot.OrderId;
            Address = photoShoot.Address;
            Start = photoShoot.Start;
            DurationMinutes = (uint) photoShoot.Duration.Minutes;

            Images = withReferences && photoShoot.Images != null
                ? photoShoot.Images.Select(image => new OutputPhotoShootImage(image, false))
                : null;
        }

        public Guid Id { get; }

        public int OrderId { get; }

        public string Address { get; }

        public DateTime Start { get; }

        public uint DurationMinutes { get; }

        public IEnumerable<OutputPhotoShootImage> Images { get; }
    }
}