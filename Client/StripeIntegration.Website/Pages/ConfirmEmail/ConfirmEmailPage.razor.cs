using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Website.Pages.ConfirmEmail;

public partial class ConfirmEmailPage
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }

    private bool _successVerified = true;

    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryStrings = QueryHelpers.ParseQuery(uri.Query);

        var model = new ConfirmEmailModel();

        if (queryStrings.TryGetValue("email", out var email)) model.Email = email;
        if (queryStrings.TryGetValue("code", out var code)) model.Code = code;

        var response = await AuthenticationService.ConfirmEmailAsync(model);

        _successVerified = response;
        StateHasChanged();
    }
}
