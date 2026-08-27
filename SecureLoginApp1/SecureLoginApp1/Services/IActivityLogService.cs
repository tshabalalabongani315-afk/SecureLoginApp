using System.Collections.Generic;
using System.Threading.Tasks;
using SecureLoginApp1.Models;

namespace SecureLoginApp1.Services
{
    public interface IActivityLogService
    {
        Task LogAsync(string userId, string type, string description);

        Task<List<ActivityLog>> GetRecentAsync(string userId, int count = 10);
    }
}
