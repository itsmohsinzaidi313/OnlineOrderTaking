using System.Security.Cryptography;

namespace PointofSaleModels
{
    public static class TokenGenerator
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateToken(int halfLength = 4)
        {
            if (halfLength <= 0)
                throw new ArgumentException("Length must be positive.", nameof(halfLength));

            return $"{GenerateSegment(halfLength)}-{GenerateSegment(halfLength)}";
        }

        private static string GenerateSegment(int length)
        {
            var bytes = new byte[length];
            var chars = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            for (int i = 0; i < length; i++)
            {
                chars[i] = Characters[bytes[i] % Characters.Length];
            }

            return new string(chars);
        }
    }
}
