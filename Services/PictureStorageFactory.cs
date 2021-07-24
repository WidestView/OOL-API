using System;
using System.IO;

namespace OOL_API.Services
{
    public static class PictureStorageFactory
    {
        public static Func<IServiceProvider, IPictureStorage<TModel, TKey>>
            StorageOf<TModel, TKey>(
                string directory,
                Func<TModel, TKey> identifierOf,
                Func<TKey, string> identifierToString = null
            )
        {
            directory = directory ?? throw new ArgumentNullException(nameof(directory));
            identifierOf = identifierOf ?? throw new ArgumentNullException(nameof(identifierOf));
            identifierToString ??= key => key.ToString();

            return provider => new FunctionPictureStorage<TModel, TKey>(
                (IPictureStorageDelegate) provider.GetService(typeof(IPictureStorageDelegate)),
                new FunctionPictureStorage<TModel, TKey>.Config
                {
                    Directory = directory,
                    IdentifierOf = identifierOf,
                    IdentifierToString = identifierToString
                }
            );
        }
    }
    public class FunctionPictureStorage<TModel, TKey> : IPictureStorage<TModel, TKey>
    {
        public class Config
        {
            public string Directory { get; set; }

            public Func<TModel, TKey> IdentifierOf { get; set; }

            public Func<TKey, string> IdentifierToString { get; set; }
        }

        private readonly IPictureStorageDelegate _delegate;

        private readonly Config _config;

        public FunctionPictureStorage(
            IPictureStorageDelegate storage,
            Config config
        )
        {
            _delegate = storage;
            _delegate.Directory = config.Directory;
            _config = config;
        }

        public byte[] GetPicture(TKey id)
            => _delegate.GetPicture(_config.IdentifierToString(id));

        public void PostPicture(Stream stream, TModel image)
            => _delegate.PostPicture(
                stream, _config.IdentifierToString(_config.IdentifierOf(image))
            );
    }

}