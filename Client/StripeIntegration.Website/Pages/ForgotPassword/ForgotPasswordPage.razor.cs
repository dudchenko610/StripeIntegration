using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.Shared.Helpers;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Website.Pages.ForgotPassword;

public partial class ForgotPasswordPage
{
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly ForgotPasswordModel _forgotPasswordRequestModel = new();
    private bool _forgetPasswordLoading = false;
    private ServerException? _serverException;

    private async Task OnForgotPasswordAsync()
    {
        if (_forgetPasswordLoading) return;
        _forgetPasswordLoading = true;

        if (!EmailHelper.IsValidEmail(_forgotPasswordRequestModel.Email!))
        {
            _serverException = new ServerException
            {
                ErrorMessages = ["Incorrect email!"],
                ServerErrorType = ServerErrorType.IncorrectEmail
            };

            StateHasChanged();
            return;
        }

        var result = await AuthenticationService.ForgotPasswordAsync(_forgotPasswordRequestModel);

        _forgetPasswordLoading = false;

        if (result.Result != null)
        {
            NavigationManager.NavigateTo(Constants.ClientRoutes.SignIn);
        }

        _serverException = result.Exception;
        StateHasChanged();
    }

    private void OnKeyDown()
    {
        if (_serverException is null) return;
        _serverException = null;
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
}
