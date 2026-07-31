using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the LoginModel class.
    /// </summary>
    /// <param name="signInManager">The SignInManager for authenticating users.</param>
    /// <param name="userService">The UserService for updating user LastLogin.</param>
    public LoginModel(SignInManager<ApplicationUser> signInManager, IUserService userService)
    {
        _signInManager = signInManager;
        _userService = userService;
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
            // Best-effort update of LastLogin; do not block sign-in if it fails
            var user = await _userService.GetCurrentUserAsync(User);
            if (user != null)
            {
                await _userService.UpdateLastLoginAsync(user);
            }

            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}
