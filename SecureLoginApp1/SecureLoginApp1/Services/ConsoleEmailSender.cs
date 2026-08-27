using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Development-time <see cref="IEmailSender"/> that logs the message instead of sending it,
    /// so confirmation/reset links are visible in the console without a real SMTP server.
    /// </summary>
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            _logger.LogInformation(
                "\n===== DEV EMAIL =====\nTo: {ToEmail}\nSubject: {Subject}\n{Body}\n======================",
                toEmail, subject, htmlBody);
            return Task.CompletedTask;
        }
    }
}
