using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IActionResult> ListImages(CancellationToken token = default)
        {
            return Ok((await _context.PhotoShootImages.Select(img => img.Id).ToListAsync(token))
                .Select(id =>
                    new
                    {
                        id,
                        url = $"{ApiUrl}{Url.Action(action: "GetImageContent", values: new {id})}"
                    }
                ));
        }

#endif

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageContent(Guid id, CancellationToken token = default)
        {
            var content = await _pictureStorage.GetPicture(id, token);

            if (content != null)
            {
                return File(content, contentType: "image/jpeg");
            }

            return NotFound();
        }

        [HttpGet("data/{id}")]
        public async Task<IActionResult> GetImageData(Guid id)
        {
            var image = await _context.PhotoShootImages
                .Include(img => img.PhotoShoot)
                .FirstOrDefaultAsync(img => img.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(
                new OutputPhotoShootImage(image, withReferences: true)
            );
        }


        [HttpPost("upload/{photoShootResourceId}")]
        public async Task<IActionResult> UploadImage(Guid photoShootResourceId, [FromForm] IFormFile file)
        {
            if (IPictureStorageInfo.IsSupported(file.ContentType))
            {
                return StatusCode(400);
            }

            var photoShoot = await FindPhotoShoot(photoShootResourceId);

            if (photoShoot == null)
            {
                return NotFound();
            }

            var image = NewImageFromPhotoShoot(photoShoot);

            await RegisterPhotoShootImage(image);

            await SavePhotoShootImage(file, image);

            return CreatedAtAction(
                actionName: "GetImageData",
                routeValues: new {id = image.Id},
                value: new OutputPhotoShootImage(image, withReferences: true)
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

        private async Task SavePhotoShootImage(IFormFile file, PhotoShootImage image)
        {
            await using var stream = file.OpenReadStream();

            await _pictureStorage.PostPicture(stream, image);
        }

        private async Task RegisterPhotoShootImage(PhotoShootImage image)
        {
            await _context.PhotoShootImages.AddAsync(image);

            await _context.SaveChangesAsync();

            Debug.Assert(image.Id != Guid.Empty);
        }

        private async Task<PhotoShoot> FindPhotoShoot(Guid photoShootResourceId)
        {
            return await _context.PhotoShoots.FirstOrDefaultAsync(shoot => shoot.ResourceId == photoShootResourceId);
        }
    }
}
