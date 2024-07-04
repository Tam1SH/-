using System.Text;
using System.Security.Cryptography;

namespace back.Utils
{
    public static class Cryptography
    {
        public static string sha256(string input)
        {
            using var crypt = SHA256.Create();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
