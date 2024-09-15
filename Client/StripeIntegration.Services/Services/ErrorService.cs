using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Exceptions;

namespace StripeIntegration.Service.Services;

public class ErrorService : IErrorService
{
    public event Action<ServerException>? OnErrorCame;

    public void SendError(ServerException exception)
    {
        OnErrorCame?.Invoke(exception);
    }
}