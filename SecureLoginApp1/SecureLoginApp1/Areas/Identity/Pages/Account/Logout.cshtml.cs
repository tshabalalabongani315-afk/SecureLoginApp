using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        // Provide a confirmation message to the next request
        TempData["StatusMessage"] = "You have been logged out.";
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToPage("/Index", new { area = "" });
        }
    }
}
