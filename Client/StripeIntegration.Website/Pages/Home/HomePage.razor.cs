using Microsoft.AspNetCore.Components;

namespace StripeIntegration.Website.Pages.Home;

public partial class HomePage
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
}