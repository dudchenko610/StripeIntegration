using Microsoft.AspNetCore.Components;

namespace StripeIntegration.Website.Pages.PaymentError;

public partial class PaymentErrorPage
{
    [Inject] public required NavigationManager NavigationManager { get; set; }

    [Parameter] public required string SessionId { get; set; }
}