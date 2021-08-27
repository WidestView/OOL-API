using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OOL_API.Services
{
    public interface IPasswordHash
    {
        public string Of(string password);
    }

    public class Sha256PasswordHash : IPasswordHash
    {
        public string Of(string password)
        {
            using var sha = SHA256.Create();

            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            return string.Join("", bytes.Select(b => b.ToString("x2")));
        }
    }
}