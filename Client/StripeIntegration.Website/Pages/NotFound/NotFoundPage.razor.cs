using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StripeIntegration.Website.Pages.NotFound;

public partial class NotFoundPage
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    private async Task GoBackAsync()
    {
        await JsRuntime.InvokeVoidAsync("goBack");
    }
}