using System;
using System.Security.Cryptography;
using System.Text;

namespace G3.Services {
    public class HashService : IHashService {
        private readonly Random random = new();

        public string HashPassword(string plaintext) {
            return BCrypt.Net.BCrypt.HashPassword(plaintext);
        }

        public bool Verify(string plaintext, string hash) {
            return BCrypt.Net.BCrypt.Verify(plaintext, hash);
        }

        public string RandomHash() {
            using SHA256 sha256 = SHA256.Create();
            string randomString = RandomStringGenerator(32);
            byte[] inputBytes = Encoding.UTF8.GetBytes(randomString);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hashBytes) {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public string RandomStringGenerator(int length) {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuilder = new(length);

            for (int i = 0; i < length; i++) {
                int index = random.Next(characters.Length);
                stringBuilder.Append(characters[index]);
            }

            return stringBuilder.ToString();
        }
    }
}

