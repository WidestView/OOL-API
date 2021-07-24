using System;
using System.Collections.Generic;
using System.Linq;
using OOL_API.Data;
using OOL_API.Models;

namespace OOL_API.Services
{
    public class PhotoShootPictureStorage : AbstractPictureStorage<PhotoShootImage, Guid>
    {
        private readonly StudioContext _context;

        public PhotoShootPictureStorage(
            StudioContext context,
            IPictureStorageDelegate storage
        )
            : base(storage, "PhotoShootImages")
            => _context = context;

        protected override Guid IdentifierOf(PhotoShootImage model) 
            => model.Id;

        public override IEnumerable<Guid> ListIdentifiers() 
            => _context.PhotoShootImages.Select(IdentifierOf);
    }
}