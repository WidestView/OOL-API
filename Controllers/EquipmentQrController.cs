using Microsoft.AspNetCore.Mvc;
using OOL_API.Data;
using OOL_API.Services;

namespace OOL_API.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentQrController : ControllerBase
    {
        private readonly StudioContext _context;
        private readonly QrHandler _handler;

        public EquipmentQrController(QrHandler handler, StudioContext context)
        {
            _handler = handler;
            _context = context;
        }

        [HttpGet]
        [Route("qr/{id}")]
        public IActionResult GetQr(int id)
        {
            var equipment = _context.Equipments.Find(id);

            if (equipment == null) return NotFound();

            return _handler.GenerateQrFor(equipment).ToFileResult();
        }
    }
}