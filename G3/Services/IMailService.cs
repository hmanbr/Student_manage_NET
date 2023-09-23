namespace G3.Services {
    public interface IMailService {
        public string? GetDomain(string email);
        public void SendMailConfirm(string email, string hash);
    }
}

