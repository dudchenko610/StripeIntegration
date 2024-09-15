using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.Shared.Helpers;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Website.Pages.SignUp;

public partial class SignUpPage
{
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    
    [Parameter] public required string Email { get; set; }

    private readonly SignUpModel _signUpRequestModel = new();
    private bool _showPassword;
    private bool _signUpLoading;
    private string _signInSocialLoading = string.Empty;
    private bool _showAdditionalPassword;
    private bool _showWindow;
    private string _repeatedPassword = string.Empty;
    private ServerException? _serverException;

    private bool _licenceAccepted = false;
        
    protected override void OnInitialized()
    {
        _signUpRequestModel.Email = Email;
        _signUpRequestModel.SocialSignUp = !string.IsNullOrEmpty(_signUpRequestModel.Email);

        StateHasChanged();
    }

    private async Task OnSignUpAsync()
    {
        if (_signUpLoading) return;
        _signUpLoading = true;

        if (!EmailHelper.IsValidEmail(_signUpRequestModel.Email!))
        {
            _serverException = new ServerException
            {
                ErrorMessages = ["Incorrect email!"],
                ServerErrorType = ServerErrorType.IncorrectEmail
            };

            _signUpLoading = false;
            StateHasChanged();
            return;
        }

        if (_signUpRequestModel.Password != _repeatedPassword)
        {
            _signUpRequestModel.Password = "";
            _repeatedPassword = "";
            _serverException = new ServerException
            {
                ErrorMessages = new List<string> { "Passwords do not match!" },
                ServerErrorType = ServerErrorType.PasswordsDoNotMatch
            };

            StateHasChanged();
            return;
        }

        var result = await AuthenticationService.SignUpAsync(_signUpRequestModel);

        _signUpLoading = false;

        if (result.Result is not null)
        {
            _showWindow = true;
            StateHasChanged();
            return;
        }

        _serverException = result.Exception;
        StateHasChanged();
    }
    
    private async Task OnSocialClickedAsync(string provider)
    {
        if (!string.IsNullOrEmpty(_signInSocialLoading) || _signUpLoading) return;
        _signInSocialLoading = provider;
        StateHasChanged();
        
        var url = await AuthenticationService.GetSocialUrlAsync(provider);

        _signInSocialLoading = string.Empty;
        
        if (!string.IsNullOrWhiteSpace(url))
        {
            NavigationManager.NavigateTo(url);
            return;
        }

        StateHasChanged();
    }

    private void OnKeyDown()
    {
        if (_serverException is null) return;
        _serverException = null;
    }

    private string GetErrorBackground()
    {
        if (_serverException == null!)
            return "";

        return _serverException.ServerErrorType 
            is ServerErrorType.UserAlreadyExists 
            or ServerErrorType.RegistrationFailed 
            or ServerErrorType.EmptyEmail 
            or ServerErrorType.EmptyName ? "#F7F1F1" : "";
    }
    
    private string GetEmailErrorBackground()
    {
        if (_serverException == null!)
            return "";

        return _serverException.ServerErrorType 
            is ServerErrorType.UserAlreadyExists 
            or ServerErrorType.RegistrationFailed 
            or ServerErrorType.EmptyEmail 
            or ServerErrorType.EmptyName
            or ServerErrorType.IncorrectEmail ? "#F7F1F1" : "";
    }
    
    private string GetPasswordsErrorBackground()
    {
        if (_serverException == null!)
            return "";

        return _serverException.ServerErrorType 
            is ServerErrorType.UserAlreadyExists 
            or ServerErrorType.RegistrationFailed 
            or ServerErrorType.EmptyEmail 
            or ServerErrorType.EmptyName
            or ServerErrorType.PasswordsDoNotMatch ? "#F7F1F1" : "";
    }
}
