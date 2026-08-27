using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SecureLoginApp1.Exceptions;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Sends email via SMTP using the configured <see cref="SmtpSettings"/>.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Host, _settings.Port,
                    _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                if (!string.IsNullOrEmpty(_settings.Username))
                {
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                }
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new EmailDeliveryException($"Failed to send email to {toEmail}.", ex);
            }
        }
    }
}
