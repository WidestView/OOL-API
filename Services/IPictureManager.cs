using System.Collections.Generic;
using System.IO;

namespace OOL_API.Services
{
    public interface IPictureManager
    {
        public IEnumerable<string> ListIdentifiers();
        public byte[] GetPicture(string identifier);

        public string PostPicture(Stream content, string name = null);
    }
}