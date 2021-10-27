using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                storage: (IPictureStorageDelegate) provider.GetService(typeof(IPictureStorageDelegate)),
                config: new FunctionPictureStorage<TModel, TKey>.Config
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
        private readonly Config _config;

        private readonly IPictureStorageDelegate _delegate;

        public FunctionPictureStorage(
            IPictureStorageDelegate storage,
            Config config
        )
        {
            _delegate = storage;
            _delegate.Directory = config.Directory;
            _config = config;
        }

        public Task<byte[]> GetPicture(TKey id, CancellationToken token = default)
        {
            return _delegate.GetPicture(id: _config.IdentifierToString(id), token);
        }

        public Task PostPicture(Stream stream, TModel image)
        {
            return _delegate.PostPicture(
                stream, model: _config.IdentifierToString(_config.IdentifierOf(image))
            );
        }

        public class Config
        {
            public string Directory { get; set; }

            public Func<TModel, TKey> IdentifierOf { get; set; }

            public Func<TKey, string> IdentifierToString { get; set; }
        }
    }
}
