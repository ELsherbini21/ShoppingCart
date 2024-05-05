using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Helpers.interfaces;
using ShoppingCart.PL.Helpers.Models;

namespace ShoppingCart.PL.Helpers
{
    public class EmailSettings : IEmailSettings
    {
        private readonly MailSettings _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            // i want to get value that exist in mail settings . 
            _options = options.Value;
        }

        public void SendEmail(Email email)
        {
            var mailMimeMessage = new MimeMessage
            {
                // I will get Sender Of Email From appSettings.json
                Sender = MailboxAddress.Parse(_options.Email),

                Subject = email.Subject,
            };

            mailMimeMessage.To.Add(MailboxAddress.Parse(email.To));

            // i don't want to Display Email At Mail_Box , I want to Display Name only
            // => i make new MailBoxAddress ,, Because want to display name only.
            mailMimeMessage.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            // I will Get the body Of Email 
            var builder = new BodyBuilder();

            builder.TextBody = email.Body;

            mailMimeMessage.Body = builder.ToMessageBody(); //Because it's Accept only , Type [Body Builder]


            //configure The Connection 
            using var smtpConnection = new SmtpClient();

            // Secure Socket => depend on Tls [Port]
            smtpConnection.Connect(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls);

            // i will make Auth using Email And Password 
            smtpConnection.Authenticate(_options.Email, _options.Password);

            smtpConnection.Send(mailMimeMessage);

            smtpConnection.Disconnect(true);

        }

    }
}
