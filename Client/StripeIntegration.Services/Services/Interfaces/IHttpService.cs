using StripeIntegration.ViewModels.Models;

namespace StripeIntegration.Service.Services.Interfaces;

public interface IHttpService
{
    Task<ApiResponse<TResult>> PostAsync<TResult, TRequest>(string uriPrefix, TRequest request, bool showAuthError = true)
        where TResult : class, new()
        where TRequest : class;
    
    Task<ApiResponse<TResult>> PostAsync<TResult>(string uriPrefix, bool showAuthError = true)
        where TResult : class, new();

    Task<ApiResponse<TResult>> GetAsync<TResult>(string uriPrefix = "", bool showAuthError = true)
        where TResult : class, new();
}