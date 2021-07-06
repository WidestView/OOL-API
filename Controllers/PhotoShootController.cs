using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    public class PhotoShootController : Controller
    {
        // todo: use proper url retrieval method
        private const string ApiUrl = "http://localhost:5000";

        private readonly PhotoShootPictureManager _pictureManager;

        private readonly StudioContext _context;

        public PhotoShootController(
            PhotoShootPictureManager pictureManager,
            StudioContext context
        )
            => (_pictureManager, _context) = (pictureManager, context);

        // todo: remove this on production
        
        // GET: api/photoshoot/images
        [Route("photoshoot/images")]
        public IActionResult ListAll()
            => Json(_pictureManager.ListIdentifiers().Select(id =>
                new
                {
                    id,
                    url = $"{ApiUrl}{Url.Action("GetImageData", new {id})}"
                }
            ));

        [Route("photoshoot/imagedata/{id}")]
        public IActionResult GetImageData(string id)
        {
            throw new NotImplementedException();
        }

        [Route("photoshoot/image/{id}")]
        public IActionResult GetContent(string id)
        {
            var content = _pictureManager.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }
        
        [Route("photoshoot/upload/{photoShootResourceId}")]
        [HttpPost]
        public IActionResult Add(Guid photoShootResourceId, [FromForm] IFormFile file)
        {
            if (file?.ContentType != "image/jpeg")
            {
                return StatusCode(400);
            }

            var photoShoot =
                _context.PhotoShoots.FirstOrDefault(shoot => shoot.ResourceId == photoShootResourceId);

            if (photoShoot == null)
            {
                return NotFound();
            }

            var image = new PhotoShootImage
            {
                PhotoShoot =  photoShoot,
                PhotoShootId = photoShoot.Id
            };

            _context.PhotoShootImages.Add(image);

            Debug.Assert(image.Id != Guid.Empty);

            using var stream = file.OpenReadStream();
            
            _pictureManager.PostPicture(stream, image);

            return Json(new { id = image.Id.ToString() });
        }
    }
}