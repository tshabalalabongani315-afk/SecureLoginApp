namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Builds the otpauth:// URI an authenticator app scans to add a TOTP account.
    /// Kept separate from UserManager calls so the URI format itself is unit-testable in isolation.
    /// </summary>
    public interface ITwoFactorService
    {
        string GenerateQrCodeUri(string email, string unformattedKey);
    }
}
