using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [Route("photoshoot")]
    public class PhotoShootController : Controller
    {
        // todo: use proper url retrieval method
        private const string ApiUrl = "http://localhost:5000";

        private readonly PhotoShootPictureStorage _pictureStorage;

        private readonly StudioContext _context;

        public PhotoShootController(
            PhotoShootPictureStorage pictureStorage,
            StudioContext context
        )
            => (_pictureStorage, _context) = (pictureStorage, context);

        // todo: remove this on production
        
        [HttpGet("images")]
        public IActionResult ListImages()
            => Json(_pictureStorage.ListIdentifiers().Select(id =>
                new
                {
                    id,
                    url = $"{ApiUrl}{Url.Action("GetImageData", new {id})}"
                }
            ));

        [HttpGet("imagedata/{id}")]
        public IActionResult GetImageData(string id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("image/{id}")]
        public IActionResult GetImageContent(Guid id)
        {
            var content = _pictureStorage.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }
        
        [HttpPost("upload/{photoShootResourceId}")]
        public IActionResult UploadImage(Guid photoShootResourceId, [FromForm] IFormFile file)
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

            _context.SaveChanges();

            Debug.Assert(image.Id != Guid.Empty);

            using var stream = file.OpenReadStream();
            
            _pictureStorage.PostPicture(stream, image);

            return Json(new { id = image.Id.ToString() });
        }

        [HttpGet]
        public ActionResult<IEnumerable<PhotoShoot>> ListAll()
        {
            return _context.PhotoShoots;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _context
                .PhotoShoots
                .FirstOrDefault(shoot => shoot.ResourceId == id);

            if (result == null)
            {
                return NotFound();
            }

            return Json(result);
        }

        [HttpPost("add")]
        public IActionResult Add(PhotoShoot shoot)
        {
            if (ModelState.IsValid)
            {
                _context.PhotoShoots.Add(shoot);

                return CreatedAtAction("GetById", new {id = shoot.ResourceId});
            }

            return StatusCode(400);
        }
    }
}