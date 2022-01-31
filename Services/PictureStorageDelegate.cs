using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace OOL_API.Services
{
    public interface IPictureStorageDelegate : IPictureStorage<string, string>
    {
        public string Directory { set; }
    }

    public class PictureStorageDelegate : IPictureStorageDelegate
    {
        private readonly IWebHostEnvironment _environment;

        private string _directory;

        public PictureStorageDelegate(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string Directory
        {
            set
            {
                _directory = Path.Combine(path1: "Images", value);

                System.IO.Directory.CreateDirectory(_directory);
            }
        }

        public async Task<byte[]> GetPicture(string id, CancellationToken token = default)
        {
            var path = ResolveImagePath(id);

            try
            {
                return await File.ReadAllBytesAsync(path, token);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File {path} recognized but not found");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Directory asked for {path} not found");
            }

            return null;
        }

        public async Task PostPicture(Stream stream, string id)
        {
            var path = ResolveImagePath(id);

            await using var file = File.OpenWrite(path);

            file.Seek(offset: 0, SeekOrigin.Begin);

            await stream.CopyToAsync(file);
        }

        private string ResolveImagePath(string image)
        {
            if (_directory == null)
            {
                throw new InvalidOperationException(
                    "The image directory is not configured"
                );
            }

            return Path.Combine(_environment.ContentRootPath, _directory, path3: $"{image}.jpg");
        }
    }
}
