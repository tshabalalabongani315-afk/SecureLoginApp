using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using SecureUserPortal.Tests.TestHelpers;
using Xunit;

namespace SecureUserPortal.Tests
{
    public class PasswordResetTests
    {
        [Fact]
        public async Task ForgotPassword_UnknownEmail_StillRedirectsToConfirmation_WithoutSendingEmail()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            userManagerMock.Setup(m => m.FindByEmailAsync("nobody@example.com")).ReturnsAsync((ApplicationUser?)null);
            var emailSenderMock = new Mock<IEmailSender>();

            var model = new ForgotPasswordModel(userManagerMock.Object, emailSenderMock.Object)
            {
                Input = new ForgotPasswordModel.InputModel { Email = "nobody@example.com" }
            };
            PageModelTestSetup.Attach(model);

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./ForgotPasswordConfirmation", redirect.PageName);
            emailSenderMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_KnownEmail_SendsResetLink_AndRedirectsToSameConfirmation()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "known@example.com" };
            userManagerMock.Setup(m => m.FindByEmailAsync("known@example.com")).ReturnsAsync(user);
            userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");
            var emailSenderMock = new Mock<IEmailSender>();

            var model = new ForgotPasswordModel(userManagerMock.Object, emailSenderMock.Object)
            {
                Input = new ForgotPasswordModel.InputModel { Email = "known@example.com" }
            };
            PageModelTestSetup.Attach(model);

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./ForgotPasswordConfirmation", redirect.PageName);
            emailSenderMock.Verify(e => e.SendEmailAsync("known@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_Success_InvalidatesOldPassword_AndRedirectsToLogin()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            var user = new ApplicationUser { Id = "user-1", Email = "known@example.com" };
            userManagerMock.Setup(m => m.FindByEmailAsync("known@example.com")).ReturnsAsync(user);
            userManagerMock
                .Setup(m => m.ResetPasswordAsync(user, "real-token", "NewP@ssword1"))
                .ReturnsAsync(IdentityResult.Success);

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("real-token"));

            var model = new ResetPasswordModel(userManagerMock.Object)
            {
                Input = new ResetPasswordModel.InputModel
                {
                    Email = "known@example.com",
                    Password = "NewP@ssword1",
                    ConfirmPassword = "NewP@ssword1",
                    Code = encodedCode
                }
            };
            PageModelTestSetup.Attach(model);

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Account/Login", redirect.PageName);
            userManagerMock.Verify(m => m.ResetPasswordAsync(user, "real-token", "NewP@ssword1"), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_UnknownEmail_DoesNotRevealNonExistence_StillRedirectsToLogin()
        {
            var userManagerMock = MockUserManagerFactory.Create();
            userManagerMock.Setup(m => m.FindByEmailAsync("nobody@example.com")).ReturnsAsync((ApplicationUser?)null);

            var model = new ResetPasswordModel(userManagerMock.Object)
            {
                Input = new ResetPasswordModel.InputModel
                {
                    Email = "nobody@example.com",
                    Password = "NewP@ssword1",
                    ConfirmPassword = "NewP@ssword1",
                    Code = "irrelevant"
                }
            };
            PageModelTestSetup.Attach(model);

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Account/Login", redirect.PageName);
        }
    }
}
