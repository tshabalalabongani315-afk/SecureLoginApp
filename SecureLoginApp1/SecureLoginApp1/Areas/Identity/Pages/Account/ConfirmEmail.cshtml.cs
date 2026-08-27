using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SecureLoginApp1.Models;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public bool Succeeded { get; private set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            Succeeded = false;
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Succeeded = false;
            return Page();
        }

        // A malformed or already-used token should render the failure state, not throw to the view.
        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            Succeeded = result.Succeeded;
        }
        catch
        {
            Succeeded = false;
        }

        if (Succeeded)
        {
            TempData["StatusMessage"] = "Thank you for confirming your email.";
        }

        return Page();
    }
}
