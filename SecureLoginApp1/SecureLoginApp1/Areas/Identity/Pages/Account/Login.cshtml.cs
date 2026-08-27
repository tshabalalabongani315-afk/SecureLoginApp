using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;
using SecureLoginApp1.Models.Events;
using SecureLoginApp1.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of the LoginModel class.
    /// </summary>
    /// <param name="signInManager">The SignInManager for authenticating users.</param>
    /// <param name="userService">The UserService for updating user LastLogin.</param>
    /// <param name="eventPublisher">Publishes domain events (e.g. login) for activity logging.</param>
    public LoginModel(SignInManager<ApplicationUser> signInManager, IUserService userService, IEventPublisher eventPublisher)
    {
        _signInManager = signInManager;
        _userService = userService;
        _eventPublisher = eventPublisher;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        // Default redirect to Dashboard instead of home
        returnUrl ??= Url.Content("~/Dashboard");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            // Best-effort update of LastLogin; find user by email (User principal may not be populated yet in same request)
            try
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    await _userService.UpdateLastLoginAsync(user);
                    await _eventPublisher.PublishAsync(new UserLoggedInEvent(user.Id));
                }
            }
            catch
            {
                // Swallow exceptions to avoid blocking sign-in
            }

            // Set a confirmation message for successful login
            TempData["StatusMessage"] = "You have signed in successfully.";

            return LocalRedirect(returnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account is locked out.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}
