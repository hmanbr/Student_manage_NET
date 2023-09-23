
namespace G3.Services {
    public interface IHashService {
        public string HashPassword(string plaintext);
        public bool Verify(string plaintext, string hash);
        public string RandomHash();
        public string RandomStringGenerator(int length);
    }
}

