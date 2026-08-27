using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using SecureUserPortal.Tests.TestHelpers;
using Xunit;

namespace SecureUserPortal.Tests
{
    public class EmailVerificationTests
    {
        [Fact]
        public async Task ConfirmEmail_WithMalformedToken_DoesNotThrow_AndReportsFailure()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
            userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);

            var model = new ConfirmEmailModel(userManagerMock.Object);
            PageModelTestSetup.Attach(model);

            var exception = await Record.ExceptionAsync(() => model.OnGetAsync("user-1", "%%%not-valid-base64url%%%"));

            Assert.Null(exception);
            Assert.False(model.Succeeded);
        }

        [Fact]
        public async Task ConfirmEmail_WithValidToken_Succeeds()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
            userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
            userManagerMock.Setup(m => m.ConfirmEmailAsync(user, "real-token")).ReturnsAsync(IdentityResult.Success);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("real-token"));

            var model = new ConfirmEmailModel(userManagerMock.Object);
            PageModelTestSetup.Attach(model);

            await model.OnGetAsync("user-1", encodedToken);

            Assert.True(model.Succeeded);
        }

        [Fact]
        public async Task ConfirmEmail_UnknownUserId_ReportsFailure_WithoutThrowing()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            userManagerMock.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

            var model = new ConfirmEmailModel(userManagerMock.Object);
            PageModelTestSetup.Attach(model);

            var exception = await Record.ExceptionAsync(() => model.OnGetAsync("missing", "irrelevant"));

            Assert.Null(exception);
            Assert.False(model.Succeeded);
        }

        [Fact]
        public async Task ResendConfirmation_WhenAlreadyConfirmed_DoesNotSendEmail()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com", EmailConfirmed = true };
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var userServiceMock = new Mock<IUserService>();
            var emailSenderMock = new Mock<IEmailSender>();
            var activityLogServiceMock = new Mock<IActivityLogService>();

            var model = new SecureLoginApp1.Pages.DashboardModel(userServiceMock.Object, userManagerMock.Object, emailSenderMock.Object, activityLogServiceMock.Object);
            PageModelTestSetup.Attach(model);

            await model.OnPostResendConfirmationAsync();

            emailSenderMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResendConfirmation_WhenUnconfirmed_SendsFreshTokenEmail()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com", EmailConfirmed = false };
            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            userManagerMock.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("fresh-token");

            var userServiceMock = new Mock<IUserService>();
            var emailSenderMock = new Mock<IEmailSender>();
            var activityLogServiceMock = new Mock<IActivityLogService>();

            var model = new SecureLoginApp1.Pages.DashboardModel(userServiceMock.Object, userManagerMock.Object, emailSenderMock.Object, activityLogServiceMock.Object);
            PageModelTestSetup.Attach(model);

            await model.OnPostResendConfirmationAsync();

            emailSenderMock.Verify(e => e.SendEmailAsync("test@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(m => m.GenerateEmailConfirmationTokenAsync(user), Times.Once);
        }
    }
}
