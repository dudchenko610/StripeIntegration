using StripeIntegration.ViewModels.Models;
using StripeIntegration.ViewModels.Models.Accounts;

namespace StripeIntegration.Service.Services.Interfaces;

public interface IAuthenticationService
{
    bool IsUserLoggedIn { get; }
    
    Task<ApiResponse<TokenModel>> SignInAsync(SignInModel model);
    Task<ApiResponse<TokenModel>> SignUpAsync(SignUpModel model);
    Task LogoutAsync();
    Task<bool> ConfirmEmailAsync(ConfirmEmailModel model);
    Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordModel model);
    Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordModel model);

    Task<string> GetSocialUrlAsync(string provider);
    Task<SocialSignInModel?> TrySocialSignInAsync(SocialAuthCodeModel model);
}
