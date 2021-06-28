using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    public class ImageController : Controller
    {
        private const string ApiUrl = "http://localhost:5000";

        private readonly IPictureManager _pictureManager;

        public ImageController(IPictureManager pictureManager)
            => _pictureManager = pictureManager;

        [Route("images")]
        public IActionResult ListAll()
            => Json(_pictureManager.ListIdentifiers().Select(id =>
                new
                {
                    id,
                    url = $"{ApiUrl}{Url.Action("Get", new {id})}"
                }
            ));

        [Route("images/{id}")]
        public IActionResult Get(string id)
        {
            var content = _pictureManager.GetPicture(id);

            if (content != null)
            {
                return File(content, "image/jpeg");
            }

            return NotFound();
        }
        
        [Route("images/add")]
        public IActionResult Add([FromForm] IFormFile file)
        {
            if (file?.ContentType != "image/jpeg")
            {
                return StatusCode(400);
            }

            using var stream = file.OpenReadStream();
            
            string id = _pictureManager.PostPicture(stream);
            
            return Json(new { id });
        }
    }
}