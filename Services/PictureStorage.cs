using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OOL_API.Services
{
    public interface IPictureStorage<in TModel, in TKey>
    {
        public Task<byte[]> GetPicture(TKey id, CancellationToken token = default);

        public Task PostPicture(Stream stream, TModel model);
    }

    public interface IPictureStorageInfo
    {
        public static bool IsSupported(string contentType)
        {
            return contentType == "image/jpeg";
        }
    }
}
