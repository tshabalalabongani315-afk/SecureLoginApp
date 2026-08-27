using System.Threading.Tasks;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Sends transactional email (confirmation links, password resets) on behalf of the app.
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
