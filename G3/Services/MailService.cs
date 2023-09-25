using MailKit.Net.Smtp;
using MimeKit;

namespace G3.Services {
    public class MailService : IMailService {
        public string? GetDomain(string email) {
            try {
                return new System.Net.Mail.MailAddress(email).Host;
            } catch (FormatException) {
                return null;
            }
        }

        public void SendMailConfirm(string email, string hash) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", "tunahe140525@fpt.edu.vn"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Email Confirmation";

            message.Body = new TextPart("plain") {
                Text = "https://localhost:7200/auth/confirm/" + hash,
            };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("tunahe140525@fpt.edu.vn", "wmmzcmllqvjnfrul");
            client.Send(message);
            client.Disconnect(true);
        }

        public void SendResetPassword(string email, string hash)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", "tunahe140525@fpt.edu.vn"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Reset password";

            message.Body = new TextPart("plain") {
                Text = "https://localhost:7200/auth/reset-password/" + hash,
            };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("tunahe140525@fpt.edu.vn", "wmmzcmllqvjnfrul");
            client.Send(message);
            client.Disconnect(true);
        }
    }
}

