using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Api.Services
{
    public class ShortCodeGenerator : IShortCodeGenerator
    {
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ShortCodeLength = 6;

        public string GenerateShortCode()
        {
            var randomBytes = new byte[ShortCodeLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var shortCode = new StringBuilder(ShortCodeLength);
            for (int i = 0; i < ShortCodeLength; i++)
            {
                shortCode.Append(AllowedChars[randomBytes[i] % AllowedChars.Length]);
            }
            return shortCode.ToString();
        }
    }
}