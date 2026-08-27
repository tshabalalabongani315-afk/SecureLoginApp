using Microsoft.AspNetCore.Identity;
using System;

namespace SecureLoginApp1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}