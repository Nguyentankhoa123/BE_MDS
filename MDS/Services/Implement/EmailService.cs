using MailKit.Net.Smtp;
using MDS.Shared.Core.Helper;
using MimeKit;

namespace MDS.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _config;
        public EmailService(EmailConfig config)
        {
            _config = config;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _config.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_config.SmtpServer, _config.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_config.UserName, _config.Password);

                client.Send(mailMessage);
            }
            catch
            {

                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
