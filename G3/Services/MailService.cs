using MailKit.Net.Smtp;
using MimeKit;

namespace G3.Services {
    public class MailService : IMailService {
        private readonly IConfiguration Configuration;

        private readonly string host;
        private readonly int port;
        private readonly bool useSsl;
        private readonly string emailSender;
        private readonly string password;
        private readonly string url;

        public MailService(IConfiguration configuration) {
            Configuration = configuration;

            host = Configuration["MailSetting:Host"];
            port = int.Parse(Configuration["MailSetting:Port"]);
            useSsl = bool.Parse(Configuration["MailSetting:UseSsl"]);
            emailSender = Configuration["MailSetting:Email"];
            password = Configuration["MailSetting:Password"];
            url = Configuration["MailSetting:Url"];
        }

        
        public void SendMailConfirm(string emailReceiver, string hash) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", emailSender));
            message.To.Add(new MailboxAddress(emailReceiver, emailReceiver));
            message.Subject = "Email Confirmation";
            message.Body = new TextPart("plain") { Text = url + "/auth/confirm/" + hash };

            using var client = new SmtpClient();
            client.Connect(host, port, useSsl);
            client.Authenticate(emailSender, password);
            client.Send(message);
            client.Disconnect(true);
        }

        public void SendResetPassword(string emailReceiver, string hash) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", emailSender));
            message.To.Add(new MailboxAddress(emailReceiver, emailReceiver));
            message.Subject = "Reset Password";
            message.Body = new TextPart("plain") { Text = url + "/auth/reset-password/" + hash };

            using var client = new SmtpClient();
            client.Connect(host, port, useSsl);
            client.Authenticate(emailSender, password);
            client.Send(message);
            client.Disconnect(true);
        }

        public string? GetDomain(string email) {
            try {
                return new System.Net.Mail.MailAddress(email).Host;
            } catch (FormatException) {
                return null;
            }
        }

        public string? GetAddress(string email) {
            try {
                return new System.Net.Mail.MailAddress(email).User;
            } catch (FormatException) {
                return null;
            }
        }
        public void SendPassword(string email, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", emailSender));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Email Confirmation";
            message.Body = new TextPart("plain") { Text = "Your password is: " + password };

            using var client = new SmtpClient();
            client.Connect(host, port, useSsl);
            client.Authenticate(emailSender, this.password);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}

