using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PackagePictureStorage
    {
        private readonly IWebHostEnvironment _environment;
        
        private readonly StudioContext _context;

        private readonly string _directory;

        public PackagePictureStorage(
            StudioContext context,
            IWebHostEnvironment environment,
            string directory = null
        )
        {
            _directory = directory ?? Path.Join("Images", "Package_Images").ToString();
            _environment = environment;
            _context = context;
        }

        public IEnumerable<Guid> ListIdentifiers() 
            => _context.PhotoShootImages.Select(image => image.Id);

        public byte[] GetPicture(int id)
        {
            Package package = _context.Packages.Find(id);

            if (package != null)
            {
                var path = ResolveImagePath(id.ToString());
                
                try
                {
                    return File.ReadAllBytes(path);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"File {path} recognized but not found");
                }
            }

            return null;
        }
        /*
        public void PostPicture(Stream stream)
        {
        }
        */
        private string ResolveImagePath(string image)
            => Path.Combine(_environment.ContentRootPath, $"{_directory}\\{image}.jpg");
    }
}