using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Models.DataTransfer;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [Route("photoshoot/images")]
    public class PhotoShootImageController : Controller
    {
        // todo: use proper url retrieval method
        private const string ApiUrl = "http://localhost:5000";

        private readonly PhotoShootPictureStorage _pictureStorage;

        private readonly StudioContext _context;

        public PhotoShootImageController(
            PhotoShootPictureStorage pictureStorage,
            StudioContext context
        ) => (_pictureStorage, _context) = (pictureStorage, context);

#if DEBUG

        [HttpGet]
        public IActionResult ListImages()
            => Json(_pictureStorage.ListIdentifiers().Select(id =>
                new
                {
                    id,
                    url = $"{ApiUrl}{Url.Action("GetImageContent", new {id})}"
                }
            ));

#endif

        [HttpGet("{id}")]
        public IActionResult GetImageContent(Guid id)
        {
            var content = _pictureStorage.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }

        [HttpGet("data/{id}")]
        public IActionResult GetImageData(Guid id)
        {
            var image = _context.PhotoShootImages
                .Include(img => img.PhotoShoot)
                .FirstOrDefault(img => img.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return Json(
                new OutputPhotoShootImage(image, true)
            );
        }


        [HttpPost("upload/{photoShootResourceId}")]
        public IActionResult UploadImage(Guid photoShootResourceId, [FromForm] IFormFile file)
        {
            if (!IsValid(file))
            {
                return StatusCode(400);
            }

            var photoShoot = FindPhotoShoot(photoShootResourceId);

            if (photoShoot == null)
            {
                return NotFound();
            }

            var image = NewImageFromPhotoShoot(photoShoot);

            RegisterPhotoShootImage(image);

            SavePhotoShootImage(file, image);

            return CreatedAtAction(
                "GetImageData",
                new {id = image.Id},
                new OutputPhotoShootImage(image, true)
            );
        }

        private static PhotoShootImage NewImageFromPhotoShoot(PhotoShoot photoShoot)
        {
            var image = new PhotoShootImage
            {
                PhotoShoot = photoShoot,
                PhotoShootId = photoShoot.Id
            };
            return image;
        }

        private void SavePhotoShootImage(IFormFile file, PhotoShootImage image)
        {
            using var stream = file.OpenReadStream();

            _pictureStorage.PostPicture(stream, image);
        }

        private void RegisterPhotoShootImage(PhotoShootImage image)
        {
            _context.PhotoShootImages.Add(image);

            _context.SaveChanges();

            Debug.Assert(image.Id != Guid.Empty);
        }

        private PhotoShoot FindPhotoShoot(Guid photoShootResourceId)
        {
            return _context.PhotoShoots.FirstOrDefault(shoot => shoot.ResourceId == photoShootResourceId);
        }

        private static bool IsValid(IFormFile file)
        {
            return file?.ContentType == "image/jpeg";
        }
    }
}