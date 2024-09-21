using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.Shared.Helpers;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Website.Pages.SignIn;

public partial class SignInPage
{
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }
    [Inject] public required  NavigationManager NavigationManager { get; set; }

    private readonly SignInModel _signInRequestModel = new();
    private bool _showPassword;
    private bool _signInLoading;
    private string _signInSocialLoading = string.Empty;

    private ServerException? _serverException;

    private async Task OnSignInAsync()
    {
        if (!string.IsNullOrEmpty(_signInSocialLoading) || _signInLoading) return;

        _signInLoading = true;
        StateHasChanged();

        if (!EmailHelper.IsValidEmail(_signInRequestModel.Email!))
        {
            _serverException = new ServerException
            {
                ErrorMessages = ["Incorrect email!"],
                ServerErrorType = ServerErrorType.IncorrectEmail
            };

            _signInLoading = false;
            StateHasChanged();
            return;
        }

        var result = await AuthenticationService.SignInAsync(_signInRequestModel);
        
        _signInLoading = false;

        if (result.Result != null!)
        {
            NavigationManager.NavigateTo(Constants.ClientRoutes.Profile);
            return;
        }

        _serverException = result.Exception;
        StateHasChanged();
    }

    private async Task OnSocialClickedAsync(string provider)
    {
        // if(!string.IsNullOrEmpty(_signInSocialLoading) || _signInLoading) return;
        //
        // _signInSocialLoading = provider;
        // StateHasChanged();
        //
        // var url = await AuthenticationService.GetSocialUrlAsync(provider);
        //
        // _signInSocialLoading = string.Empty;
        //
        // if (!string.IsNullOrWhiteSpace(url))
        // {
        //     NavigationManager.NavigateTo(url);
        //     return;
        // }
        //
        // StateHasChanged();
    }

    private void OnKeyDown()
    {
        if (_serverException is null) return;
        _serverException = null;
    }

    private string GetErrorBackground()
    {
        if (_serverException == null) return "";

        return _serverException.ServerErrorType 
            is ServerErrorType.WrongLoginOrPassword 
            or ServerErrorType.UserNotFound 
            or ServerErrorType.UserNotConfirmed
            or ServerErrorType.EmptyEmail 
            or ServerErrorType.EmptyName ? "#F7F1F1" : "";
    }
    
    private string GetEmailErrorBackground()
    {
        if (_serverException is null) return "";

        return _serverException.ServerErrorType 
            is ServerErrorType.UserAlreadyExists 
            or ServerErrorType.RegistrationFailed 
            or ServerErrorType.EmptyEmail 
            or ServerErrorType.EmptyName
            or ServerErrorType.IncorrectEmail ? "#F7F1F1" : "";
    }
}