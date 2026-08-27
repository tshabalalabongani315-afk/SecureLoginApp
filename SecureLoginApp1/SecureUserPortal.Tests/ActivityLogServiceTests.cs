using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SecureLoginApp1.Models.Events;
using SecureLoginApp1.Services;
using SecureLoginApp1.Services.EventHandlers;
using Xunit;

namespace SecureUserPortal.Tests
{
    public class ActivityLogServiceTests
    {
        [Fact]
        public async Task PublishingUserLoggedInEvent_LogsExactlyOneRow()
        {
            var activityLogServiceMock = new Mock<IActivityLogService>();
            var handler = new UserLoggedInActivityHandler(activityLogServiceMock.Object);

            await handler.HandleAsync(new UserLoggedInEvent("user-1"));

            activityLogServiceMock.Verify(
                s => s.LogAsync("user-1", "Login", It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishingPasswordChangedEvent_LogsExactlyOneRow()
        {
            var activityLogServiceMock = new Mock<IActivityLogService>();
            var handler = new PasswordChangedActivityHandler(activityLogServiceMock.Object);

            await handler.HandleAsync(new PasswordChangedEvent("user-1"));

            activityLogServiceMock.Verify(
                s => s.LogAsync("user-1", "PasswordChanged", It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishingProfileUpdatedEvent_LogsExactlyOneRow()
        {
            var activityLogServiceMock = new Mock<IActivityLogService>();
            var handler = new ProfileUpdatedActivityHandler(activityLogServiceMock.Object);

            await handler.HandleAsync(new ProfileUpdatedEvent("user-1"));

            activityLogServiceMock.Verify(
                s => s.LogAsync("user-1", "ProfileUpdated", It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task InMemoryEventPublisher_InvokesOnlyHandlersRegisteredForThatEventType()
        {
            var services = new ServiceCollection();
            var activityLogServiceMock = new Mock<IActivityLogService>();
            services.AddSingleton(activityLogServiceMock.Object);
            services.AddScoped<IEventHandler<UserLoggedInEvent>, UserLoggedInActivityHandler>();

            var provider = services.BuildServiceProvider();
            var publisher = new InMemoryEventPublisher(provider);

            await publisher.PublishAsync(new UserLoggedInEvent("user-1"));

            activityLogServiceMock.Verify(s => s.LogAsync("user-1", "Login", It.IsAny<string>()), Times.Once);

            // No handler registered for PasswordChangedEvent — publishing it must not throw or log anything.
            await publisher.PublishAsync(new PasswordChangedEvent("user-1"));
            activityLogServiceMock.Verify(s => s.LogAsync(It.IsAny<string>(), "PasswordChanged", It.IsAny<string>()), Times.Never);
        }
    }
}
