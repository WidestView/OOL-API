using System.Collections.Generic;
using System.IO;
using System.Linq;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PackagePictureStorage : IPictureStorage<Package, int>
    {
        private readonly DirectoryPictureStorage _directoryStorage;

        private readonly StudioContext _context;

        public PackagePictureStorage(
            DirectoryPictureStorage directoryStorage,
            StudioContext context
        )
        {
            _directoryStorage = directoryStorage;
            _context = context;
        }

        public IEnumerable<int> ListIdentifiers()
        {
            return _context.Packages.Select(p => p.ID);
        }

        public byte[] GetPicture(int id)
        {
            return _directoryStorage.GetPicture(id.ToString());
        }

        public void PostPicture(Stream stream, Package image)
        {
            _directoryStorage.PostPicture(stream, image.ID.ToString());
        }
    }
}