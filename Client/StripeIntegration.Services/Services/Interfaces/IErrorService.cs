using StripeIntegration.Shared.Exceptions;

namespace StripeIntegration.Service.Services.Interfaces;

public interface IErrorService
{
    event Action<ServerException>? OnErrorCame;
    void SendError(ServerException exception);
}