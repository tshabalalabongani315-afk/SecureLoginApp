using System;

namespace SecureLoginApp1.Models
{
    /// <summary>
    /// A single recorded user action, shown on the Dashboard's activity timeline.
    /// </summary>
    public class ActivityLog
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
