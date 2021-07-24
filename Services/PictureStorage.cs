using System.IO;

namespace OOL_API.Services
{
    public interface IPictureStorage<in TModel, in TKey>
    {
        public byte[] GetPicture(TKey id);

        public void PostPicture(Stream stream, TModel model);
    }

}