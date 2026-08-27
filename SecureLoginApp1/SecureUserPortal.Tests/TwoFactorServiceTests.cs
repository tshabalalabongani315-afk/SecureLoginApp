using SecureLoginApp1.Services;
using Xunit;

namespace SecureUserPortal.Tests
{
    public class TwoFactorServiceTests
    {
        [Fact]
        public void GenerateQrCodeUri_ProducesWellFormedOtpAuthUri()
        {
            var service = new TwoFactorService();

            var uri = service.GenerateQrCodeUri("user@example.com", "JBSWY3DPEHPK3PXP");

            Assert.StartsWith("otpauth://totp/", uri);
            Assert.Contains("secret=JBSWY3DPEHPK3PXP", uri);
            Assert.Contains("issuer=SecureUserPortal", uri);
            Assert.Contains(System.Uri.EscapeDataString("user@example.com"), uri);
        }

        [Fact]
        public void GenerateQrCodeUri_EscapesSpecialCharactersInEmail()
        {
            var service = new TwoFactorService();

            var uri = service.GenerateQrCodeUri("user+test@example.com", "SECRETKEY");

            Assert.DoesNotContain("user+test@example.com", uri);
            Assert.Contains(System.Uri.EscapeDataString("user+test@example.com"), uri);
        }
    }
}
