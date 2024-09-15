using AspNet.OAuth.Providers.Models;
using StripeIntegration.ViewModels.Models.Accounts;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.BLL.Services.Interfaces;

public interface IAccountService
{
    Task<SocialSignInModel> TrySocialSignInAsync(SocialUserModel model);
    Task<TokenModel> SignInAsync(SignInModel model);
    Task<TokenModel> SignUpAsync(SignUpModel model);
    Task UpdateAsync(UpdateUserModel model);
    Task<bool> ConfirmEmailAsync(ConfirmEmailModel model);
    Task ForgotPasswordAsync(ForgotPasswordModel model);
    Task<UserModel> GetCurrentUserAsync();
    Task<TokenModel> UpdateTokensAsync(TokenModel model);
    Task ChangePasswordAsync(ChangePasswordModel model);
}
