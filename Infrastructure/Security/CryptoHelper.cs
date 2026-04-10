using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public static class CryptoHelper
    {
        public static (string hash, string salt) HashWithRandomSalt(string input)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);
            var hash = ComputeSha256($"{salt}:{input}");
            return (hash, salt);
        }

        public static string ComputeSha256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifySha256(string input, string salt, string expectedHash)
        {
            var actual = ComputeSha256($"{salt}:{input}");
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(actual),
                Convert.FromBase64String(expectedHash));
        }

        public static string GenerateSecureToken(int bytesLength = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(bytesLength);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}

