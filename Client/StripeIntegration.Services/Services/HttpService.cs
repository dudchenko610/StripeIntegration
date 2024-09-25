using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using StripeIntegration.Service.Providers;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Models;

namespace StripeIntegration.Service.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IErrorService _errorService;
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    class TokenModel 
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
    
    public HttpService(
        HttpClient httpClient,
        IErrorService errorService,
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authStateProvider,
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _errorService = errorService;
        _localStorageService = localStorageService;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public Task<ApiResponse<TResult>> PostAsync<TResult>(string uriPrefix, bool showAuthError = true) where TResult : class, new()
    {
        return PostAsync<TResult, object>(uriPrefix, new object(), showAuthError);
    }

    public async Task<ApiResponse<TResult>> GetAsync<TResult>(string uriPrefix = "", bool showAuthError = true) where TResult : class, new()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uriPrefix);

        var dateTimeNow = DateTime.Now;
        
        request.Headers.Add("X-Id", GenerateCode(dateTimeNow));
        request.Headers.Add("Time", dateTimeNow.ToString(CultureInfo.InvariantCulture));

        var firstResponse = await SendAsync<TResult>(request, showAuthError);

        if (firstResponse.IsTokenRefreshed)
        {
            request = new HttpRequestMessage(HttpMethod.Get, uriPrefix);

            var secondResponse = await SendAsync<TResult>(request, showAuthError);
            
            return new ApiResponse<TResult>
            {
                Result = secondResponse.Result,
                Exception = secondResponse.Exception
            };
        }

        return new ApiResponse<TResult>
        {
            Result = firstResponse.Result,
            Exception = firstResponse.Exception
        };
    }

    public async Task<ApiResponse<TResult>> PostAsync<TResult, TRequest>(string uriPrefix, TRequest body, bool showAuthError = true)
        where TResult : class, new()
        where TRequest : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uriPrefix);
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
  
        var dateTimeNow = DateTime.Now;
        
        request.Headers.Add("X-Id", GenerateCode(dateTimeNow));
        request.Headers.Add("Time", dateTimeNow.ToString(CultureInfo.InvariantCulture));

        var firstResponse = await SendAsync<TResult>(request, showAuthError);

        if (firstResponse.IsTokenRefreshed)
        {
            request = new HttpRequestMessage(HttpMethod.Post, uriPrefix);
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var secondResponse = await SendAsync<TResult>(request, showAuthError);
            
            return new ApiResponse<TResult>
            {
                Result = secondResponse.Result,
                Exception = secondResponse.Exception
            };
        }

        return new ApiResponse<TResult>
        {
            Result = firstResponse.Result,
            Exception = firstResponse.Exception
        };
    }

    private async Task<(TResult Result, ServerException Exception, bool IsTokenRefreshed)> SendAsync<TResult>(HttpRequestMessage request, 
        bool showAuthError = false)
        where TResult : class, new()
    {
        var token = await _localStorageService.GetItemAsync<string>(Constants.Auth.AccessToken);
        token ??= string.Empty;
        
        // Console.WriteLine($"token = {token}");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<TResult>(content)!, null!, false);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var tokens = await UpdateRefreshAsync();

                if (tokens != null!) return (null!, null!, true);

                var ex1 = new ServerException("Error occured, re-login please!", ServerErrorType.General,
                    response.StatusCode);
                
                if (showAuthError) _errorService.SendError(ex1);

                await _localStorageService.SetItemAsync(Constants.Auth.AccessToken, "");
                ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
                if (showAuthError) _navigationManager.NavigateTo(Constants.ClientRoutes.Subscriptions);
                
                return (null!, ex1, false);
            }

            var errorJson = await response.Content.ReadAsStringAsync();
            var serverException = JsonConvert.DeserializeObject<ServerException>(errorJson);

            var ex2 = serverException ?? new ServerException
            {
                Code = response.StatusCode,
                ServerErrorType = ServerErrorType.General
            };
            
            if (showAuthError) _errorService.SendError(ex2);

            return (null!, ex2, false);
        }
        catch (Exception ex)
        {
            var ex3 = new ServerException
            {
                Code = HttpStatusCode.InternalServerError,
                ServerErrorType = ServerErrorType.General,
                ErrorMessages = new List<string> { ex.Message }
            };
            
            if (showAuthError) _errorService.SendError(ex3);
            
            return (null!, ex3, false);
        }
    }

    private async Task<TokenModel?> UpdateRefreshAsync()
    {
        var accessToken = await _localStorageService.GetItemAsync<string>(Constants.Auth.AccessToken);
        var refreshToken = await _localStorageService.GetItemAsync<string>(Constants.Auth.RefreshToken);

        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var request = new HttpRequestMessage(HttpMethod.Post, "api/account/update-tokens");
        request.Content = new StringContent(JsonConvert.SerializeObject(new { AccessToken = accessToken, RefreshToken = refreshToken }), Encoding.UTF8, "application/json");

        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TokenModel>(content)!;

            await _localStorageService.SetItemAsync(Constants.Auth.AccessToken, result.AccessToken);
            await _localStorageService.SetItemAsync(Constants.Auth.RefreshToken, result.RefreshToken);

            return result;
        }
        catch (Exception ex)
        {
            // ignored
        }

        return null;
    }

    private static string GenerateCode(DateTime date)
    {
        var guild = Guid.NewGuid().ToString()[..^2];
        var randomValue = date.Second;
        var res = guild + randomValue.ToString("X2").ToLower();

        Console.WriteLine(res);
        
        return res;
    }
}