using System.Collections.Generic;
using System.IO;

namespace OOL_API.Services
{
    public interface IPictureStorageDelegate
    {
        public byte[] GetPicture(string id);

        public void PostPicture(Stream stream, string id);
        
        public string Directory { get; set; }
    }

    public interface IPictureStorage<in T, TIdentifier>
    {
        public IEnumerable<TIdentifier> ListIdentifiers();

        public byte[] GetPicture(TIdentifier id);

        public void PostPicture(Stream stream, T image);
        
    }
    public abstract class AbstractPictureStorage<T, TIdentifier> 
        : IPictureStorage<T, TIdentifier>
    {
        private readonly IPictureStorageDelegate _delegate;
        
        protected AbstractPictureStorage(
            IPictureStorageDelegate storage,
            string directory
        )
        {
            _delegate = storage;
            _delegate.Directory = directory;
        }

        protected virtual string IdentifierToString(TIdentifier id) 
            => id.ToString();

        protected abstract TIdentifier IdentifierOf(T model);
        
        public abstract IEnumerable<TIdentifier> ListIdentifiers();

        public byte[] GetPicture(TIdentifier id)
        {
            return _delegate.GetPicture(IdentifierToString(id));
        }

        public void PostPicture(Stream stream, T image)
        {
            _delegate.PostPicture(stream, IdentifierToString(IdentifierOf(image)));
        }
    }
}