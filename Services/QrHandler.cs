using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using OOL_API.Models;
using QRCoder;

namespace OOL_API.Services
{
    public class QrHandler
    {
        private readonly QRCodeGenerator _generator;

        public QrHandler(QRCodeGenerator generator)
        {
            _generator = generator;
        }

        public ImageResult GenerateQrFor(Equipment equipment)
        {
            equipment = equipment ?? throw new ArgumentException(nameof(equipment));

            var id = equipment.Id.ToString();

            var text = "oolMobile://" +
                       JsonSerializer.Serialize(
                           new
                           {
                               type = "equipment",
                               id
                           }
                       );

            var data = _generator.CreateQrCode(
                text,
                QRCodeGenerator.ECCLevel.M
            );

            var qr = new QRCode(data);

            var qrImage = qr.GetGraphic(20);

            return GetImageResult(qrImage);
        }

        private ImageResult GetImageResult(Bitmap bitmap)
        {
            using var stream = new MemoryStream();

            bitmap.Save(stream, ImageFormat.Jpeg);

            return new ImageResult(stream.ToArray(), "image/jpeg");
        }
    }
}