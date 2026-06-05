using System.Security.Cryptography;
using System.Text;

namespace API_Usuario.Services
{
    public class HashService
    {
        public static string ComputeSha256(string text)
        {
            using var sha256 = SHA256.Create();

            var bytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes(text));

            return Convert.ToHexString(bytes);
        }
    }
}
