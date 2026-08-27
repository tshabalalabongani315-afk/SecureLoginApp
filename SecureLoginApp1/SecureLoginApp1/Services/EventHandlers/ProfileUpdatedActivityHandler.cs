using System.Threading.Tasks;
using SecureLoginApp1.Models.Events;

namespace SecureLoginApp1.Services.EventHandlers
{
    public class ProfileUpdatedActivityHandler : IEventHandler<ProfileUpdatedEvent>
    {
        private readonly IActivityLogService _activityLogService;

        public ProfileUpdatedActivityHandler(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public Task HandleAsync(ProfileUpdatedEvent domainEvent) =>
            _activityLogService.LogAsync(domainEvent.UserId, "ProfileUpdated", "Profile information updated.");
    }
}
