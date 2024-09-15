using StripeIntegration.Shared.Exceptions;

namespace StripeIntegration.ViewModels.Models;

public class ApiResponse<TResult> where TResult : class
{
    public TResult? Result { get; set; }
    public ServerException? Exception { get; set; }
}