using System;

namespace SecureLoginApp1.ViewModels
{
    /// <summary>
    /// Display-shaped activity entry for the Dashboard timeline; keeps the raw
    /// <see cref="Models.ActivityLog"/> entity out of the view.
    /// </summary>
    public record ActivityLogEntryViewModel(string Title, DateTime? TimestampUtc, string? Details = null);
}
