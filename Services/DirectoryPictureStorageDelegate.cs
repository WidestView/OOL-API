using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OOL_API.Services
{
    public class DirectoryPictureStorageDelegate : IPictureStorageDelegate
    {
        private readonly IWebHostEnvironment _environment;

        private string _directory;

        public string Directory
        {
            get => _directory;
            set => _directory = Path.Join("Images", value);
        }

        public DirectoryPictureStorageDelegate(IWebHostEnvironment environment)
            => _environment = environment;

        public byte[] GetPicture(string id)
        {
            var path = ResolveImagePath(id);

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

            return null;
        }

        public void PostPicture(Stream stream, string id)
        {
            var path = ResolveImagePath(id);

            using var file = File.OpenWrite(path);

            file.Seek(0, SeekOrigin.Begin);

            stream.CopyTo(file);
        }

        private string ResolveImagePath(string image)
        {
            if (_directory == null)
            {
                throw new InvalidOperationException(
                    "The image directory is not configured"
                );
            }

            return Path.Combine(_environment.ContentRootPath, $"{_directory}\\{image}.jpg");
        }
    }

}