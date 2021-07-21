using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PhotoShootPictureStorage
    {
        private readonly IWebHostEnvironment _environment;
        
        private readonly StudioContext _context;

        private readonly string _directory;

        public PhotoShootPictureStorage(
            StudioContext context,
            IWebHostEnvironment environment,
            string directory = null
        )
        {
            _directory = directory ?? Path.Join("Images", "Photoshoot_Images").ToString();
            _environment = environment;
            _context = context;
        }

        public IEnumerable<Guid> ListIdentifiers() 
            => _context.PhotoShootImages.Select(image => image.Id);

        public byte[] GetPicture(Guid identifier)
        {
            var image = _context.PhotoShootImages.Find(identifier);

            if (image != null)
            {
                var path = ResolveImagePath(image.Id.ToString());

                try
                {
                    return File.ReadAllBytes(path);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"File {path} recognized but not found");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Directory solicited for {path} not found");
                }
            }

            return null;
        }

        public void PostPicture(Stream stream, PhotoShootImage image)
        {
            var path = ResolveImagePath(image.Id.ToString());

            using var file = File.OpenWrite(path);

            file.Seek(0, SeekOrigin.Begin);

            stream.CopyTo(file);
        }

        private string ResolveImagePath(string image)
            => Path.Combine(_environment.ContentRootPath, $"{_directory}\\{image}.jpg");
    }
}