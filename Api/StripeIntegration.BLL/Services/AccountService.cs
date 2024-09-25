using System.Net;
using System.Security.Claims;
using System.Text;
using AspNet.OAuth.Providers.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using StripeIntegration.BLL.Providers;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.Entities.Entities;
using StripeIntegration.Shared.Enums;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Models.Accounts;
using StripeIntegration.ViewModels.Models.Email;
using StripeIntegration.ViewModels.Models.Users;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtProvider _jwtProvider;
    private readonly EmailProvider _emailProvider;
    private readonly IUserRepository _userRepository;
    private readonly IOptions<ClientConnectionOptions> _clientOptions;
    private readonly EmailConnectionOptions _emailOptions;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStripeService _stripeService;

    private UserModel? _currentUserModel = null!;

    public AccountService(
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        UserManager<User> userManager,
        JwtProvider jwtProvider,
        EmailProvider emailProvider,
        IUserRepository userRepository,
        IOptions<ClientConnectionOptions> clientOptions,
        IOptions<EmailConnectionOptions> emailOptions,
        IStripeService stripeService)
    {
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _userRepository = userRepository;
        _emailProvider = emailProvider;
        _clientOptions = clientOptions;
        _emailOptions = emailOptions.Value;
        _stripeService = stripeService;
    }

    public async Task UpdateAsync(UpdateUserModel model)
    {
        var id = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userRepository.GetByIdAsync(id);
        user.Nickname = model.Nickname;

        await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> ConfirmEmailAsync(ConfirmEmailModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        byte[] codeDecodeBytes = WebEncoders.Base64UrlDecode(model.Code!);
        string codeDecoded = Encoding.UTF8.GetString(codeDecodeBytes);

        var confirmResult = await _userManager.ConfirmEmailAsync(user, codeDecoded);
        if (!confirmResult.Succeeded)
        {
            var errorList = confirmResult.Errors.Select(x => x.Description).ToList();
            
            throw new ServerException(errorList, errorCode: HttpStatusCode.BadRequest);
        }

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task ChangePasswordAsync(ChangePasswordModel model)
    {
        if (!model.ConfirmPassword.Equals(model.ConfirmPassword))
        {
            throw new ServerException("Passwords do not match!", errorCode: HttpStatusCode.BadRequest);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            throw new ServerException("User not found!", errorCode: HttpStatusCode.BadRequest);
        }

        byte[] codeDecodeBytes = WebEncoders.Base64UrlDecode(model.Code);
        string codeDecoded = Encoding.UTF8.GetString(codeDecodeBytes);

        var result = await _userManager.ResetPasswordAsync(user, codeDecoded, model.Password);

        if (!result.Succeeded)
        {
            var errorList = result.Errors.Select(x => x.Description).ToList();
            
            throw new ServerException(errorList, errorCode: HttpStatusCode.BadRequest);
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            throw new ServerException("User not found!", errorCode: HttpStatusCode.BadRequest);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        byte[] tokenGenerateBytes = Encoding.UTF8.GetBytes(code);
        string tokenCode = WebEncoders.Base64UrlEncode(tokenGenerateBytes);

        var callbackUrl = new Uri($"{_clientOptions.Value.Url}/{_clientOptions.Value.UrlChangePassword}/{model.Email}/{tokenCode}");

        var emailSent = await _emailProvider.SendMailAsync(new MailDataModel
        {
            EmailToId = model.Email!,
            EmailToName = "Blazor Component Heap",
            EmailSubject = "Change password request",
            EmailBody = string.Format(_emailOptions.PasswordRestore, callbackUrl)
        });

        if (!emailSent) throw new ServerException("Problem with sending email to you, try again or later please");
    }

    public async Task<SocialSignInModel> TrySocialSignInAsync(SocialUserModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null) throw new ServerException("User not found!", errorCode: HttpStatusCode.BadRequest);
        
        var claims = await _jwtProvider.GetUserClaimsAsync(user.Email);
        var accessToken = _jwtProvider.GenerateAccessToken(claims);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        await _userRepository.UpdateAsync(user);

        return new SocialSignInModel
        {
            Email = model.Email,
            TokenModel = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<TokenModel> SignInAsync(SignInModel model)
    {
        if(string.IsNullOrWhiteSpace(model.Email))
            throw new ServerException("Email cannot be empty!", ServerErrorType.EmptyEmail, errorCode: HttpStatusCode.BadRequest);
        
        if(string.IsNullOrWhiteSpace(model.Password))
            throw new ServerException("Password cannot be empty!", ServerErrorType.EmptyPassword, errorCode: HttpStatusCode.BadRequest);
        
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
            throw new ServerException("User not found!", ServerErrorType.UserNotFound, errorCode: HttpStatusCode.BadRequest);

        if (!user.EmailConfirmed)
            throw new ServerException("User not confirmed!", ServerErrorType.UserNotConfirmed, errorCode: HttpStatusCode.BadRequest);

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
            throw new ServerException("Wrong login or password!", ServerErrorType.WrongLoginOrPassword, errorCode: HttpStatusCode.BadRequest);

        var claims = await _jwtProvider.GetUserClaimsAsync(user.Email);
        var accessToken = _jwtProvider.GenerateAccessToken(claims);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        await _userRepository.UpdateAsync(user);

        var tokens = new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return tokens;
    }

    public async Task<TokenModel> SignUpAsync(SignUpModel model)
    {
        if(string.IsNullOrWhiteSpace(model.Email))
            throw new ServerException("Email cannot be empty!", ServerErrorType.EmptyEmail, errorCode: HttpStatusCode.BadRequest);
        
        if(string.IsNullOrWhiteSpace(model.Nickname))
            throw new ServerException("Name cannot be empty!", ServerErrorType.EmptyName, errorCode: HttpStatusCode.BadRequest);
        
        if(model.Nickname.Length == 50)
            throw new ServerException("Name length cannot be more than 50 symbols!", ServerErrorType.EmptyName, errorCode: HttpStatusCode.BadRequest);
        
        if(string.IsNullOrWhiteSpace(model.Password))
            throw new ServerException("Password cannot be empty!", ServerErrorType.EmptyPassword, errorCode: HttpStatusCode.BadRequest);
        
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        
        if (existingUser is not null)
            throw new ServerException("User already exist!", ServerErrorType.UserAlreadyExists, errorCode: HttpStatusCode.BadRequest);
        
        var user = new User
        {
            Nickname = model.Nickname,
            UserName = model.Email,
            Email = model.Email,
            RegistrationTime = DateTime.Now.ToUniversalTime(),
            StripeCustomerId = await _stripeService.CreateCustomerAsync(model.Email!),
            EmailConfirmed = model.SocialSignUp
        };
        
        if (string.IsNullOrEmpty(user.StripeCustomerId))
            throw new ServerException("Error occurred, please try later!");
        
        var newUserResult = await _userManager.CreateAsync(user, model.Password);

        if (!newUserResult.Succeeded)
        {
            var errorList = newUserResult.Errors
                .Select(x => x.Description).ToList();
            
            throw new ServerException(errorList, ServerErrorType.RegistrationFailed, errorCode: HttpStatusCode.BadRequest);
        }
        
        if (!await SendMessageToEmailAsync(model.Email)) // TODO: cancel changes
        {
            // await _userManager.DeleteAsync(user);
            throw new ServerException("Problem with registering you in system, try later please!");
        }
        
        if (model.SocialSignUp)
        {
            var claims = await _jwtProvider.GetUserClaimsAsync(user.Email!);
            var accessToken = _jwtProvider.GenerateAccessToken(claims);
            var refreshToken = _jwtProvider.GenerateRefreshToken();
        
            user.RefreshToken = refreshToken;
        
            await _userRepository.UpdateAsync(user);
            
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        return new TokenModel();
    }

    public async Task<UserModel> GetCurrentUserAsync()
    {
        if (_currentUserModel is not null) return _currentUserModel;
        
        var id = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userRepository.GetByIdAsync(id);
        _currentUserModel = _mapper.Map<UserModel>(user);

        return _currentUserModel;
    }

    private async Task<bool> SendMessageToEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        byte[] tokenGenerateBytes = Encoding.UTF8.GetBytes(code);
        string tokenCode = WebEncoders.Base64UrlEncode(tokenGenerateBytes);

        var callbackUrl = new StringBuilder();
        callbackUrl.Append($"{_clientOptions.Value.Url}{_clientOptions.Value.UrlConfirmEmail}");
        callbackUrl.Append($"?email={email}&code={tokenCode}");

        return await _emailProvider.SendMailAsync(new MailDataModel
        {
            EmailToId = email,
            EmailToName = "Blazor Component Heap",
            EmailSubject = "Confirm your email",
            EmailBody = string.Format(_emailOptions.EmailConfirmation, callbackUrl)
        });
    }

    public async Task<TokenModel> UpdateTokensAsync(TokenModel model)
    {
        if (model is null)
        {
            throw new ServerException("Not correct data!");
        }

        var refreshToken = model.RefreshToken;

        var principal = _jwtProvider.ValidateToken(model.AccessToken);
        var userName = principal.Identity!.Name!;

        var user = await _userManager.FindByNameAsync(userName);

        if (userName is null || !user.RefreshToken.Equals(refreshToken))
        {
            throw new ServerException("Please, repeat login!");
        }

        var newAccessToken = _jwtProvider.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userRepository.UpdateAsync(user);

        var newTokens = new TokenModel
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return newTokens;
    }
}
