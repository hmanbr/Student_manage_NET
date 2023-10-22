namespace G3.Services {
    public interface IMailService {
        public string? GetDomain(string email);
        public string? GetAddress(string email);
        public void SendMailConfirm(string email, string hash);
        public void SendResetPassword(string email, string hash);
        public void SendPassword(string email, string password);
    }
}

