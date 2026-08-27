using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using SecureLoginApp1.Services.Storage;
using SecureUserPortal.Tests.TestHelpers;
using Xunit;

namespace SecureUserPortal.Tests
{
    // Validation lives in ProfileModel per the "PageModels: validation and orchestration only,
    // business logic lives in services" rule, so these exercise it through the page rather than
    // the storage service directly — the thing under test is "rejected before touching disk".
    public class ProfileImageUploadTests
    {
        private static ProfileModel CreateModel(
            out Mock<IUserService> userServiceMock,
            out Mock<IFileStorageService> fileStorageMock,
            ApplicationUser user)
        {
            userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            userServiceMock.Setup(s => s.UpdateUserAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);

            fileStorageMock = new Mock<IFileStorageService>();
            var eventPublisherMock = new Mock<IEventPublisher>();

            var model = new ProfileModel(userServiceMock.Object, eventPublisherMock.Object, fileStorageMock.Object)
            {
                Input = new ProfileModel.InputModel { FirstName = "Test", LastName = "User", Email = user.Email, PhoneNumber = null }
            };
            PageModelTestSetup.Attach(model);
            return model;
        }

        private static Mock<IFormFile> CreateFormFile(long length, string contentType, string fileName = "photo.jpg")
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.Length).Returns(length);
            fileMock.SetupGet(f => f.ContentType).Returns(contentType);
            fileMock.SetupGet(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[length > 0 ? 16 : 0]));
            return fileMock;
        }

        [Fact]
        public async Task OversizedImage_IsRejected_WithoutCallingStorage()
        {
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
            var model = CreateModel(out _, out var fileStorageMock, user);
            model.ProfileImage = CreateFormFile(3 * 1024 * 1024, "image/jpeg").Object;

            var result = await model.OnPostAsync();

            Assert.False(model.ModelState.IsValid);
            fileStorageMock.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task WrongContentType_IsRejected_WithoutCallingStorage()
        {
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
            var model = CreateModel(out _, out var fileStorageMock, user);
            model.ProfileImage = CreateFormFile(1024, "application/pdf", "resume.pdf").Object;

            var result = await model.OnPostAsync();

            Assert.False(model.ModelState.IsValid);
            fileStorageMock.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ValidImage_IsSaved_AndReplacesPreviousFile()
        {
            var user = new ApplicationUser { Id = "user-1", Email = "test@example.com", ProfileImageUrl = "/uploads/old.jpg" };
            var model = CreateModel(out _, out var fileStorageMock, user);
            fileStorageMock.Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("/uploads/new.jpg");
            model.ProfileImage = CreateFormFile(1024, "image/png", "photo.png").Object;

            await model.OnPostAsync();

            fileStorageMock.Verify(s => s.SaveAsync(It.IsAny<Stream>(), "photo.png", "image/png"), Times.Once);
            fileStorageMock.Verify(s => s.DeleteAsync("/uploads/old.jpg"), Times.Once);
            Assert.Equal("/uploads/new.jpg", model.ProfileImageUrl);
        }
    }
}
