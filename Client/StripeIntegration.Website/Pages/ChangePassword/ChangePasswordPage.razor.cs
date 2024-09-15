using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Website.Pages.ChangePassword;

public partial class ChangePasswordPage
{
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    [Parameter] public string Email { get; set; } = string.Empty;
    [Parameter] public string Code { get; set; } = string.Empty;

    private readonly ChangePasswordModel _changePasswordRequestModel = new()
    {
        Code = string.Empty,
        Email = string.Empty,
        Password = string.Empty,
        ConfirmPassword = string.Empty
    };
    
    private bool _showPassword = false;
    private bool _changePasswordLoading = false;
    private ServerException? _serverException;

    protected override void OnInitialized()
    {
        _changePasswordRequestModel.Code = Code;
        _changePasswordRequestModel.Email = Email;
    }

    private async Task OnForgotPasswordAsync()
    {
        if (_changePasswordLoading) return;

        if (!_changePasswordRequestModel.Password.Equals(_changePasswordRequestModel.ConfirmPassword))
        {
            _serverException = new ServerException
            {
                ErrorMessages = new List<string> { "Passwords do not match!" },
                ServerErrorType = ServerErrorType.PasswordsDoNotMatch
            };

            StateHasChanged();
            return;
        }

        _changePasswordLoading = true;

        var result = await AuthenticationService.ChangePasswordAsync(_changePasswordRequestModel);
        _changePasswordLoading = false;

        if (result.Result is not null) NavigationManager.NavigateTo(Constants.ClientRoutes.SignIn);

        _serverException = result.Exception;
        StateHasChanged();
    }

    private void OnKeyDown()
    {
        if (_serverException is null) return;
        _serverException = null;
    }

    private string GetErrorBackground()
    {
        if (_serverException is null) return "";

        return _serverException.ServerErrorType
            is ServerErrorType.WrongLoginOrPassword
            or ServerErrorType.UserNotFound
            or ServerErrorType.UserNotConfirmed
            or ServerErrorType.EmptyEmail
            or ServerErrorType.EmptyName ? "#F7F1F1" : "";
    }
}