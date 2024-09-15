using Microsoft.AspNetCore.Components;
using StripeIntegration.Shared.Exceptions;

namespace StripeIntegration.Components.Components.ErrorModal.Error;

public partial class ErrorComponent
{
    [Parameter] public required ServerException Exception { get; set; }
    [Parameter] public required EventCallback OnClose { get; set; }
}