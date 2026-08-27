using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;

[Authorize]
public class TwoFactorAuthenticationModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TwoFactorAuthenticationModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public bool Is2faEnabled { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostDisable2faAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (result.Succeeded)
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            TempData["StatusMessage"] = "Two-factor authentication has been disabled. You can re-enable it at any time.";
        }
        else
        {
            TempData["StatusMessage"] = "An error occurred disabling two-factor authentication.";
        }

        return RedirectToPage();
    }
}
