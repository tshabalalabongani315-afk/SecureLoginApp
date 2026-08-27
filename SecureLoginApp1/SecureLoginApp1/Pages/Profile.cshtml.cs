using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;
using SecureLoginApp1.Models.Events;
using SecureLoginApp1.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

/// <summary>
/// Profile page model for managing user profile information.
/// </summary>
[Authorize]
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the ProfileModel class.
    /// </summary>
    /// <param name="userService">The UserService for retrieving and updating user information.</param>
    /// <param name="eventPublisher">Publishes domain events (e.g. profile updated) for activity logging.</param>
    public ProfileModel(IUserService userService, IEventPublisher eventPublisher)
    {
        _userService = userService;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Gets or sets the input model containing profile information to update.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the success message displayed after profile update.
    /// </summary>
    public string SuccessMessage { get; set; }

    /// <summary>
    /// Input model for profile data.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address (read-only).
        /// </summary>
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Handles the OnGet HTTP method; loads current user profile data.
    /// </summary>
    public async Task OnGetAsync()
    {
        var user = await _userService.GetCurrentUserAsync(User);
        if (user != null)
        {
            Input = new InputModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
        }
    }

    /// <summary>
    /// Handles the OnPost HTTP method; updates user profile information.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userService.GetCurrentUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Index");
        }

        // Update user properties
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.PhoneNumber = Input.PhoneNumber;

        // Persist changes
        var success = await _userService.UpdateUserAsync(user);
        if (success)
        {
            try
            {
                await _eventPublisher.PublishAsync(new ProfileUpdatedEvent(user.Id));
            }
            catch
            {
                // Best-effort: a logging failure must not surface as a failed profile save.
            }

            SuccessMessage = "Your profile has been successfully updated.";
            return Page();
        }

        ModelState.AddModelError(string.Empty, "An error occurred while saving your profile. Please try again.");
        return Page();
    }
}
