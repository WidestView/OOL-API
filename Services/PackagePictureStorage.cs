using System.Collections.Generic;
using System.IO;
using System.Linq;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PackagePictureStorage : AbstractPictureStorage<Package, int>
    {
        private readonly StudioContext _context;

        public PackagePictureStorage(
            IPictureStorageDelegate storage,
            StudioContext context
        ) : base(storage, "PackageImages")
        {
            _context = context;
        }

        protected override int IdentifierOf(Package model) => model.ID;

        public override IEnumerable<int> ListIdentifiers() => _context.Packages.Select(p => p.ID);
    }
}