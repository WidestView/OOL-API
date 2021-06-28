using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace OOL_API.Services
{
    public class DirectoryPictureManager : IPictureManager
    {
        private readonly Dictionary<String, String> _fileMap;

        private readonly IWebHostEnvironment _environment;

        private readonly string _directory = "Images";

        public DirectoryPictureManager(IWebHostEnvironment environment)
        {
            _environment = environment;

            _fileMap =
                Directory.GetFiles(_directory)
                    .Select(Path.GetFileName)
                    .ToDictionary(
                        file => Guid.NewGuid().ToString(),
                        ResolveImagePath
                    );
        }

        public IEnumerable<string> ListIdentifiers()
        {
            return _fileMap.Keys;
        }

        public byte[] GetPicture(string identifier)
        {
            if (_fileMap.ContainsKey(identifier))
            {
                try
                {
                    return File.ReadAllBytes(_fileMap[identifier]);
                }
                catch (FileNotFoundException)
                {
                    _fileMap.Remove(identifier);
                }
            }

            return null;
        }

        public string PostPicture(Stream stream, string name = null)
        {
            var id = Guid.NewGuid().ToString();

            var path = ResolveImagePath($"{name ?? id}.jpg");

            using var file = File.OpenWrite(path);

            file.Seek(0, SeekOrigin.Begin);

            stream.CopyTo(file);

            _fileMap.Add(id, path);

            return id;
        }

        private string ResolveImagePath(string image)
            => Path.Combine(_environment.ContentRootPath, $"{_directory}/{image}");
    }
}