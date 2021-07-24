using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PhotoShootPictureStorage : IPictureStorage<PhotoShootImage, Guid>
    {
        private readonly StudioContext _context;

        private readonly DirectoryPictureStorage _storage;

        public PhotoShootPictureStorage(
            StudioContext context,
            DirectoryPictureStorage storage
        )
        {
            storage.Directory = "Photoshoot_Images";
            
            _context = context;
            _storage = storage;
        }

        public IEnumerable<Guid> ListIdentifiers() 
            => _context.PhotoShootImages.Select(image => image.Id);

        public byte[] GetPicture(Guid id) 
            => _storage.GetPicture(id.ToString());

        public void PostPicture(Stream stream, PhotoShootImage image) 
            => _storage.PostPicture(stream, image.Id.ToString());
    }
}