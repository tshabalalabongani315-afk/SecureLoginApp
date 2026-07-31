using Microsoft.AspNetCore.Identity;
using SecureLoginApp1.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Implementation of IUserService for managing ApplicationUser operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the UserService class.
        /// </summary>
        /// <param name="userManager">The UserManager for ApplicationUser.</param>
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves the current user from the HTTP context claims.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user.</param>
        /// <returns>The ApplicationUser object, or null if not found.</returns>
        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        /// <summary>
        /// Updates the LastLogin timestamp for the specified user.
        /// This is a best-effort operation and catches exceptions to prevent sign-in failures.
        /// </summary>
        /// <param name="user">The ApplicationUser to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateLastLoginAsync(ApplicationUser user)
        {
            try
            {
                if (user != null)
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch
            {
                // Best-effort operation; do not throw and block user sign-in
            }
        }

        /// <summary>
        /// Updates user profile information (FirstName, LastName, PhoneNumber).
        /// </summary>
        /// <param name="user">The ApplicationUser with updated properties.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if update succeeded.</returns>
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }
    }
}
