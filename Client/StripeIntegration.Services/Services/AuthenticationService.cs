using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using StripeIntegration.Service.Providers;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.ViewModels.Models;
using StripeIntegration.ViewModels.Models.Accounts;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpService _httpService;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly SocialCallbackOptions _socialCallbackOption;
    private readonly NavigationManager _navigationManager;

    public AuthenticationService(
        IHttpService httpService, 
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider,
        IOptions<SocialCallbackOptions> socialCallbackOption,
        NavigationManager navigationManager)
    {
        _httpService = httpService;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _socialCallbackOption = socialCallbackOption.Value;
        _navigationManager = navigationManager;
    }

    public bool IsUserLoggedIn
    {
        get => !string.IsNullOrWhiteSpace(_localStorage.GetItem<string>(Constants.Auth.AccessToken));
    }

    public async Task<ApiResponse<TokenModel>> SignInAsync(SignInModel model)
    {
        var signInResponse = await _httpService.PostAsync<TokenModel, SignInModel>("api/account/sign-in", model);
        if (signInResponse.Exception is not null) return signInResponse;

        await _localStorage.SetItemAsync(Constants.Auth.AccessToken, signInResponse.Result.AccessToken);

        // if (model.RememberMe)
        if (true)
            await _localStorage.SetItemAsync(Constants.Auth.RefreshToken, signInResponse.Result.RefreshToken);

        ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(model.Email!);

        return signInResponse;
    }

    public async Task<ApiResponse<TokenModel>> SignUpAsync(SignUpModel model)
    {
        var response = await _httpService.PostAsync<TokenModel, SignUpModel>("api/account/sign-up", model);

        if (response.Exception is null && 
            !string.IsNullOrWhiteSpace(response.Result?.AccessToken) && 
            !string.IsNullOrWhiteSpace(response.Result.RefreshToken))
        {
            await _localStorage.SetItemAsync(Constants.Auth.AccessToken, response.Result.AccessToken);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(model.Email!);
        }

        return response;
    }

    public async Task<bool> ConfirmEmailAsync(ConfirmEmailModel model)
    {
        var response = await _httpService.PostAsync<object, ConfirmEmailModel>("api/account/confirm-email", model);
        return response.Exception is null;
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordModel model)
    {
        var response = await _httpService.PostAsync<object, ChangePasswordModel>("api/account/change-password", model);
        return response;
    }

    public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var response = await _httpService.PostAsync<object, ForgotPasswordModel>("api/account/forgot-password", model);
        return response;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.SetItemAsync(Constants.Auth.AccessToken, "");
        ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        _navigationManager.NavigateTo(Constants.ClientRoutes.Dashboard);
    }
    
    public async Task<string> GetSocialUrlAsync(string provider)
    {
        var response = await _httpService
            .PostAsync<SocialAuthUrlModel, SocialAuthGetUrlModel>(
                "api/account/get-social-url", 
                new SocialAuthGetUrlModel
                {
                    Provider = provider,
                    ReturnUrl = provider == "Google" ? _socialCallbackOption.Google : _socialCallbackOption.Github
                });

        return response.Result?.Url ?? string.Empty;
    }
   
    public async Task<SocialSignInModel?> TrySocialSignInAsync(SocialAuthCodeModel model)
    {
        var response = await _httpService.PostAsync<SocialSignInModel, SocialAuthCodeModel>("api/account/try-social-sign-in", model);
        
        if (response?.Result?.TokenModel is null) return null;
        
        await _localStorage.SetItemAsync(Constants.Auth.AccessToken, response.Result.TokenModel.AccessToken);
        ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Result.Email!);

        return response.Result;
    }
}
