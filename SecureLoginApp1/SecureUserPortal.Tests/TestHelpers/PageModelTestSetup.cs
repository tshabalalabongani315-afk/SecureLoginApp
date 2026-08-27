using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace SecureUserPortal.Tests.TestHelpers
{
    public static class PageModelTestSetup
    {
        /// <summary>
        /// Wires up the minimum PageContext/TempData/Url plumbing a PageModel needs
        /// to run OnGet/OnPost handlers under test without a real HTTP pipeline.
        /// </summary>
        public static void Attach(PageModel model)
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
                httpContext,
                new RouteData(),
                new Microsoft.AspNetCore.Mvc.RazorPages.PageActionDescriptor());

            model.PageContext = new PageContext(actionContext);
            model.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Url.Page(...) is an extension method that delegates to IUrlHelper.RouteUrl(UrlRouteContext) —
            // that's the member Moq can actually intercept.
            var urlHelperMock = new Mock<Microsoft.AspNetCore.Mvc.IUrlHelper>();
            urlHelperMock.SetupGet(u => u.ActionContext).Returns(actionContext);
            urlHelperMock
                .Setup(u => u.RouteUrl(It.IsAny<Microsoft.AspNetCore.Mvc.Routing.UrlRouteContext>()))
                .Returns("https://localhost/Account/ConfirmEmail");
            model.Url = urlHelperMock.Object;
        }
    }
}
