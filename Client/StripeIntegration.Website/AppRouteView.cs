using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.Website.Attributes;

namespace StripeIntegration.Website;

public class AppRouteView : RouteView
{
    [Inject] public required NavigationManager NavManager { get; set; }
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }

    protected override void Render(RenderTreeBuilder builder)
    {
        var authAttribute = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute));
        var notAuthAttribute = Attribute.GetCustomAttribute(RouteData.PageType, typeof(UnauthorizedAttribute));

        var isAuthorized = AuthenticationService.IsUserLoggedIn;

        if (authAttribute is not null)
        {
            if (!isAuthorized)
            {
                NavManager.NavigateTo(Constants.ClientRoutes.SignIn);
                return;
            }
        }

        if (notAuthAttribute is not null)
        {
            if (isAuthorized)
            {
                NavManager.NavigateTo(Constants.ClientRoutes.Subscriptions);
                return;
            }
        }

        base.Render(builder);
    }
}