using System.Threading.Tasks;
using SecureLoginApp1.Models.Events;

namespace SecureLoginApp1.Services.EventHandlers
{
    public class UserLoggedInActivityHandler : IEventHandler<UserLoggedInEvent>
    {
        private readonly IActivityLogService _activityLogService;

        public UserLoggedInActivityHandler(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public Task HandleAsync(UserLoggedInEvent domainEvent) =>
            _activityLogService.LogAsync(domainEvent.UserId, "Login", "Signed in.");
    }
}
