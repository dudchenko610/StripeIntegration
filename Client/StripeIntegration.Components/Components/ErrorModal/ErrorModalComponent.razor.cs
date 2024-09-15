using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Exceptions;

namespace StripeIntegration.Components.Components.ErrorModal;

public partial class ErrorModalComponent : IDisposable
{
    [Inject] public required IErrorService ErrorService { get; set; }
    
    private bool _showError = false;
    private ServerException _exception = new();
    
    protected override void OnInitialized()
    {
        ErrorService.OnErrorCame += OnErrorCame;
    }

    public void Dispose()
    {
        ErrorService.OnErrorCame -= OnErrorCame;
    }
    
    private void OnErrorCame(ServerException error)
    {
        if (error.ServerErrorType != Shared.Enums.ServerErrorType.General) return;

        _exception = error;
        _showError = true;

        StateHasChanged();
    }
}