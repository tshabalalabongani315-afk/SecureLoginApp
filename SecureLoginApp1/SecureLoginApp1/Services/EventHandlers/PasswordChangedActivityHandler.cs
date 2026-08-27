using System.Threading.Tasks;
using SecureLoginApp1.Models.Events;

namespace SecureLoginApp1.Services.EventHandlers
{
    public class PasswordChangedActivityHandler : IEventHandler<PasswordChangedEvent>
    {
        private readonly IActivityLogService _activityLogService;

        public PasswordChangedActivityHandler(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public Task HandleAsync(PasswordChangedEvent domainEvent) =>
            _activityLogService.LogAsync(domainEvent.UserId, "PasswordChanged", "Password changed.");
    }
}
