using AspNet.OAuth.Providers.Enums;
using AspNet.OAuth.Providers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.ViewModels.Models.Accounts;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.API.Controllers;

[ApiController]
[Route(Constants.ApiRoutes.ACCOUNT)]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ISocialUserService _socialUserService;
    
    public AccountController(
        IAccountService accountService,
        ISocialUserService socialUserService)
    {
        _accountService = accountService;
        _socialUserService = socialUserService;
    }

    [HttpPost]
    [Route("try-social-sign-in")]
    public async Task<IActionResult> TrySocialSignIn(SocialAuthCodeModel model)
    {
        var socialUser = await _socialUserService.AuthorizeAsync(AuthProvider.Google, model.Code, model.ReturnUrl);
        var signInModel = await _accountService.TrySocialSignInAsync(socialUser);
        
        return Ok(signInModel);
    }

    [HttpPost]
    [Route("get-social-url")]
    public Task<IActionResult> GetSocialAuthReference(SocialAuthGetUrlModel model)
    {
        var reference = _socialUserService.GetUrl(AuthProvider.Google, model.ReturnUrl);
        
        return Task.FromResult<IActionResult>(Ok(new SocialAuthUrlModel
        {
            Url = reference
        }));
    }
    
    [HttpPost]
    [Route(Constants.ApiRoutes.SIGN_IN)]
    public async Task<IActionResult> SignIn(SignInModel model)
    {
        var tokens = await _accountService.SignInAsync(model);

        return Ok(tokens);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.SIGN_UP)]
    [Produces(Constants.CookieParams.JSON_TYPE)]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
        var tokens = await _accountService.SignUpAsync(model);

        return Ok(tokens);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.CONFIRM_EMAIL)]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel model)
    {
        var result = await _accountService.ConfirmEmailAsync(model);

        return Ok(result);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.FORGOT_PASSOWRD)]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        await _accountService.ForgotPasswordAsync(model);

        return Ok(new { });
    }

    [HttpGet]
    [Route(Constants.ApiRoutes.GET_USER)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> GetUser()
    {
        var user = await _accountService.GetCurrentUserAsync();

        return Ok(user);
    }
    
    [HttpPost]
    [Route(Constants.ApiRoutes.UPDATE_USER)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> UpdateUser(UpdateUserModel model)
    {
        await _accountService.UpdateAsync(model);

        return Ok();
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.UPDATE_TOKENS)]
    public async Task<IActionResult> UpdateTokensAsync(TokenModel model)
    {
        var newTokens = await _accountService.UpdateTokensAsync(model);

        return Ok(newTokens);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.CHANGE_PASSWORD)]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordModel model)
    {
        await _accountService.ChangePasswordAsync(model);

        return Ok(new { });
    }
}
