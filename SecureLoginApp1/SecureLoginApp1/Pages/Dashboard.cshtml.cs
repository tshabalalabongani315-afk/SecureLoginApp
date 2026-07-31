using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureLoginApp1.Models;
using SecureLoginApp1.Services;
using System;
using System.Threading.Tasks;

/// <summary>
/// Dashboard page model for displaying user account information.
/// </summary>
[Authorize]
public class DashboardModel : PageModel
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the DashboardModel class.
    /// </summary>
    /// <param name="userService">The UserService for retrieving user information.</param>
    public DashboardModel(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the full name (FirstName + LastName).
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the account creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the last login date and time.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Handles the OnGet HTTP method; loads user dashboard data.
    /// </summary>
    public async Task OnGetAsync()
    {
        var user = await _userService.GetCurrentUserAsync(User);
        if (user != null)
        {
            FirstName = user.FirstName ?? string.Empty;
            LastName = user.LastName ?? string.Empty;
            FullName = $"{FirstName} {LastName}".Trim();
            Email = user.Email ?? string.Empty;
            CreatedDate = user.CreatedDate;
            LastLogin = user.LastLogin;
        }
    }
}
