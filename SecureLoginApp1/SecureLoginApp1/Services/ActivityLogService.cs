using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureLoginApp1.Data;
using SecureLoginApp1.Models;

namespace SecureLoginApp1.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _dbContext;

        public ActivityLogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogAsync(string userId, string type, string description)
        {
            _dbContext.ActivityLogs.Add(new ActivityLog
            {
                UserId = userId,
                Type = type,
                Description = description,
                TimestampUtc = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ActivityLog>> GetRecentAsync(string userId, int count = 10)
        {
            return await _dbContext.ActivityLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.TimestampUtc)
                .Take(count)
                .ToListAsync();
        }
    }
}
