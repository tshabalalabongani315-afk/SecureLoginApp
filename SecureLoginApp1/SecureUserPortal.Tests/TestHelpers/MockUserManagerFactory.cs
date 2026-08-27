using Microsoft.AspNetCore.Identity;
using Moq;
using SecureLoginApp1.Models;

namespace SecureUserPortal.Tests.TestHelpers
{
    /// <summary>
    /// UserManager&lt;T&gt; has no interface, so tests mock the class directly via its store-only constructor.
    /// </summary>
    public static class MockUserManagerFactory
    {
        public static Mock<UserManager<ApplicationUser>> Create()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }
    }
}
