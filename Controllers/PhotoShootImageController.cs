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
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoShootImageController : Controller
    {
        // todo: use proper url retrieval method
        private const string ApiUrl = "http://localhost:5000";

        private readonly StudioContext _context;

        private readonly IPictureStorage<PhotoShootImage, Guid> _pictureStorage;

        public PhotoShootImageController(
            IPictureStorage<PhotoShootImage, Guid> pictureStorage,
            StudioContext context
        )
        {
            (_pictureStorage, _context) = (pictureStorage, context);
        }

#if DEBUG

        [HttpGet]
        public IActionResult ListImages()
        {
            return Json(_context.PhotoShootImages.Select(img => img.Id).Select(id =>
                new
                {
                    id,
                    url = $"{ApiUrl}{Url.Action("GetImageContent", new {id})}"
                }
            ));
        }

#endif

        [HttpGet("{id}")]
        public IActionResult GetImageContent(Guid id)
        {
            var content = _pictureStorage.GetPicture(id);

            if (content != null)
            {
                return File(fileContents: content, contentType: "image/jpeg");
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
                new OutputPhotoShootImage(image: image, withReferences: true)
            );
        }


        [HttpPost("upload/{photoShootResourceId}")]
        public IActionResult UploadImage(Guid photoShootResourceId, [FromForm] IFormFile file)
        {
            if (IPictureStorageInfo.IsSupported(file.ContentType))
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

            SavePhotoShootImage(file: file, image: image);

            return CreatedAtAction(
                actionName: "GetImageData",
                routeValues: new {id = image.Id},
                value: new OutputPhotoShootImage(image: image, withReferences: true)
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

            _pictureStorage.PostPicture(stream: stream, model: image);
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
    }
}
