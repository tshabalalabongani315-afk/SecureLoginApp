using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SecureLoginApp1.Pages
{
    /// <summary>
    /// Dashboard page model for displaying user account information and summary statistics.
    /// </summary>
    [Authorize]
    public class DashboardModel : PageModel
    {
        private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardModel"/> class.
        /// </summary>
        /// <param name="userService">Service used to retrieve and update user information.</param>
        /// <param name="userManager">Used to generate email confirmation tokens for the resend action.</param>
        /// <param name="emailSender">Used to resend the confirmation email.</param>
        public DashboardModel(IUserService userService, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            Activity = new List<ActivityEntry>();
        }

        /// <summary>
        /// Gets the logged-in user's first name.
        /// </summary>
        public string FirstName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the logged-in user's full name.
        /// </summary>
        public string FullName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the user's phone number.
        /// </summary>
        public string PhoneNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the account creation date (UTC).
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Gets the last login date/time (UTC) if available.
        /// </summary>
        public DateTime? LastLogin { get; private set; }

        /// <summary>
        /// Gets the profile image URL when available.
        /// </summary>
        public string? ProfileImageUrl { get; private set; }

        /// <summary>
        /// Gets whether the user's email address has been confirmed.
        /// </summary>
        public bool EmailConfirmed { get; private set; }

        /// <summary>
        /// Gets the initials to use as a placeholder avatar.
        /// </summary>
        public string Initials { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the server date/time for display purposes.
        /// </summary>
        public DateTime ServerDateTime { get; private set; }

        /// <summary>
        /// Gets the number of days the user has been a member.
        /// </summary>
        public int DaysAsMember { get; private set; }

        /// <summary>
        /// Gets a placeholder string for total successful logins.
        /// </summary>
        public string TotalSuccessfulLogins { get; private set; } = "N/A";

        /// <summary>
        /// Gets the account status label.
        /// </summary>
        public string AccountStatus { get; private set; } = "Active";

        /// <summary>
        /// Gets the authentication method label.
        /// </summary>
        public string AuthenticationMethod { get; private set; } = "ASP.NET Identity";

        /// <summary>
        /// Recent activity entries to display on the dashboard.
        /// </summary>
        public List<ActivityEntry> Activity { get; }

        /// <summary>
        /// Represents a simple activity entry.
        /// </summary>
        public record ActivityEntry(string Title, DateTime? Timestamp, string? Details = null);

        /// <summary>
        /// Handles GET requests and prepares the dashboard view model.
        /// </summary>
        public async Task OnGetAsync()
        {
            ServerDateTime = DateTime.UtcNow;

            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null)
            {
                return;
            }

            FirstName = user.FirstName ?? string.Empty;
            FullName = $"{user.FirstName} {user.LastName}".Trim();
            Email = user.Email ?? string.Empty;
            PhoneNumber = user.PhoneNumber ?? string.Empty;
            CreatedDate = user.CreatedDate;
            LastLogin = user.LastLogin;
            ProfileImageUrl = user.ProfileImageUrl;
            EmailConfirmed = user.EmailConfirmed;
            Initials = BuildInitials(user.FirstName, user.LastName);

            DaysAsMember = (int)(DateTime.UtcNow - CreatedDate).TotalDays;

            // Build activity list
            Activity.Clear();
            Activity.Add(new ActivityEntry("Account Created", CreatedDate, null));
            if (LastLogin.HasValue)
            {
                Activity.Add(new ActivityEntry("Last Login", LastLogin.Value, null));
            }
            // ProfileUpdated is not tracked explicitly; show placeholder when not available
            Activity.Add(new ActivityEntry("Profile Updated", null, "Not tracked"));
        }

        /// <summary>
        /// Resends the email confirmation link, rate-limited to once per <see cref="ResendCooldown"/>.
        /// </summary>
        public async Task<IActionResult> OnPostResendConfirmationAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage();
            }

            if (user.EmailConfirmed)
            {
                return RedirectToPage();
            }

            if (TempData["LastConfirmationResendUtc"] is string lastSentRaw &&
                DateTime.TryParse(lastSentRaw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var lastSent) &&
                DateTime.UtcNow - lastSent < ResendCooldown)
            {
                TempData["StatusMessage"] = "Please wait a minute before requesting another confirmation email.";
                return RedirectToPage();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = encodedToken },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email!,
                "Confirm your email",
                $"Please confirm your SecureUserPortal account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

            TempData["LastConfirmationResendUtc"] = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            TempData["StatusMessage"] = "Confirmation email sent. Check your inbox.";
            return RedirectToPage();
        }

        private static string BuildInitials(string? firstName, string? lastName)
        {
            var initials = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName)) initials += firstName.Trim()[0];
            if (!string.IsNullOrWhiteSpace(lastName)) initials += lastName.Trim()[0];
            return initials.ToUpperInvariant();
        }
    }
}
