using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;
using SecureLoginApp1.Models.Events;
using SecureLoginApp1.Services;

[AllowAnonymous]
public class LoginWith2faModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;
    private readonly IEventPublisher _eventPublisher;

    public LoginWith2faModel(SignInManager<ApplicationUser> signInManager, IUserService userService, IEventPublisher eventPublisher)
    {
        _signInManager = signInManager;
        _userService = userService;
        _eventPublisher = eventPublisher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool RememberMe { get; set; }

    public string ReturnUrl { get; set; } = "/";

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } = string.Empty;

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        ReturnUrl = returnUrl ?? Url.Content("~/Dashboard");
        RememberMe = rememberMe;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        returnUrl ??= Url.Content("~/Dashboard");

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

        if (result.Succeeded)
        {
            try
            {
                await _userService.UpdateLastLoginAsync(user);
                await _eventPublisher.PublishAsync(new UserLoggedInEvent(user.Id));
            }
            catch
            {
                // Best-effort: activity bookkeeping must not block a successful sign-in.
            }

            TempData["StatusMessage"] = "You have signed in successfully.";
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account is locked out.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
        return Page();
    }
}
