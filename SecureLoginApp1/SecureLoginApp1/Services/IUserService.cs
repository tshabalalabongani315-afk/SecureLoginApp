using System.Security.Claims;
using System.Threading.Tasks;
using SecureLoginApp1.Models;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Abstraction for user related operations.
    /// Keeps Identity-specific calls centralized to reduce duplication and support testing.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieve the application user for the provided principal.
        /// </summary>
        /// <param name="principal">The claims principal to resolve.</param>
        /// <returns>The <see cref="ApplicationUser"/> or null when not found.</returns>
        Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Update the user's LastLogin timestamp to UtcNow.
        /// </summary>
        /// <param name="user">The user to update.</param>
        Task UpdateLastLoginAsync(ApplicationUser user);

        /// <summary>
        /// Persist changes to the provided user object.
        /// </summary>
        /// <param name="user">The user with updated fields.</param>
        Task UpdateUserAsync(ApplicationUser user);
    }
}
