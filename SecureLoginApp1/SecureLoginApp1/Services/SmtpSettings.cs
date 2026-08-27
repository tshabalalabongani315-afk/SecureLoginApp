namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Bound from the "Smtp" configuration section. Left blank in appsettings by default;
    /// the app falls back to <see cref="ConsoleEmailSender"/> until a real host is configured.
    /// </summary>
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FromAddress { get; set; } = "no-reply@secureuserportal.local";

        public string FromName { get; set; } = "SecureUserPortal";

        public bool UseSsl { get; set; } = true;
    }
}
