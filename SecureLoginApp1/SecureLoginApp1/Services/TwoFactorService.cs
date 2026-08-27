using System;

namespace SecureLoginApp1.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        private const string Issuer = "SecureUserPortal";

        public string GenerateQrCodeUri(string email, string unformattedKey)
        {
            const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(
                format,
                Uri.EscapeDataString(Issuer),
                Uri.EscapeDataString(email),
                unformattedKey);
        }
    }
}
