using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecureLoginApp1.Pages
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
