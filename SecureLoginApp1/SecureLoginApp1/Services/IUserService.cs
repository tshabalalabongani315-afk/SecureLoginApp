using SecureLoginApp1.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Service interface for user-related operations using Identity.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves the current user from the HTTP context claims.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user.</param>
        /// <returns>The ApplicationUser object, or null if not found.</returns>
        Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user);

        /// <summary>
        /// Updates the LastLogin timestamp for the specified user.
        /// This is a best-effort operation and should not throw exceptions.
        /// </summary>
        /// <param name="user">The ApplicationUser to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateLastLoginAsync(ApplicationUser user);

        /// <summary>
        /// Updates user profile information (FirstName, LastName, PhoneNumber).
        /// </summary>
        /// <param name="user">The ApplicationUser with updated properties.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if update succeeded.</returns>
        Task<bool> UpdateUserAsync(ApplicationUser user);
    }
}
