using System;

namespace OOL_API.Models.DataTransfer
{
    public class OutputPhotoShootImage
    {
        public OutputPhotoShootImage(PhotoShootImage image, bool withReferences)
        {
            Id = image.Id;
            PhotoShoot = withReferences && image.PhotoShoot != null
                ? new OutputPhotoShoot(image.PhotoShoot, false)
                : null;
            OnPortfolio = image.OnPortfolio;
        }

        public Guid Id { get; }

        public OutputPhotoShoot PhotoShoot { get; }

        public bool OnPortfolio { get; }
    }
}