using Microsoft.AspNetCore.Components;

namespace StripeIntegration.Website.Pages.PaymentSuccess;

public partial class PaymentSuccessPage
{
    [Inject] public required NavigationManager NavigationManager { get; set; }

    [Parameter] public required string SessionId { get; set; }
}