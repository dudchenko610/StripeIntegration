using Microsoft.Extensions.Options;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.Shared.Exceptions;
using StripeIntegration.ViewModels.Models.Subscriptions;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IAccountService _accountService;
    private readonly IStripeService _stripeService;
    private readonly IUserRepository _userRepository;
    private readonly LicenseKeyOptions _licenseKeyOptions;
    private readonly IUserService _userService;
    private readonly Random _random = new Random();

    public SubscriptionService(
        IAccountService accountService,
        IStripeService stripeService,
        IUserRepository userRepository,
        IOptions<LicenseKeyOptions> licenseKeyOptions, 
        IUserService userService)
    {
        _accountService = accountService;
        _stripeService = stripeService;
        _userRepository = userRepository;
        _userService = userService;
        _licenseKeyOptions = licenseKeyOptions.Value;
    }

    public async Task<string> CheckLicenseKeyAsync(SubscriptionKeyModel model)
    {
        return string.Empty;
        // if (model == null! || string.IsNullOrEmpty(model.SubscriptionKey))
        // {
        //     return GenerateSubscriptionResponse(false);
        // }
        //
        // var applications = await _applicationRepository.GetAsync(x => x.LicenseKey == model.SubscriptionKey);
        // var application = applications.FirstOrDefault();
        //
        // if (application is null || applications.Count != 1)
        //     return GenerateSubscriptionResponse(false); // potentially problematic situation in case of second condition
        //     
        // if (application.AssemblyName != model.AssemblyName) 
        //     return GenerateSubscriptionResponse(false);
        //
        // var user = await _userRepository.GetByIdAsync(application.UserId);
        //
        // if (model.SubscriptionKey.StartsWith(Constants.Token.TRIAL_PREFIX))
        // {
        //     var subscriptionModel = await _userService.GetSubscriptionAsync(user.Id.ToString());
        //     
        //     if ((DateTime.Now - user.RegistrationTime).TotalDays <= _licenseKeyOptions.TrialKeyExpirationInDays &&
        //         subscriptionModel is not null && subscriptionModel.IsTrial) return GenerateSubscriptionResponse(true); // trial is not expired yet
        // }
        // else if (model.SubscriptionKey.StartsWith(Constants.Token.LIVE_PREFIX))
        // {
        //     var subscriptionModel = await _userService.GetSubscriptionByCustomerIdAsync(user.StripeCustomerId);
        //
        //     if (subscriptionModel is not null && subscriptionModel.Status == SubscriptionStatus.Active)
        //         return GenerateSubscriptionResponse(true);
        // }
        //
        // return GenerateSubscriptionResponse(false);
    }

    public async Task<PaymentSessionModel> GetPaymentSessionAsync(GetPaymentSessionModel model)
    {
        var currentSubscription = await _userService.GetSubscriptionAsync();

        if (currentSubscription is not null && !currentSubscription.IsTrial)
        {
            await _stripeService.CancelSubscriptionAsync(currentSubscription.Id);
        }

        var user = await _accountService.GetCurrentUserAsync();
        var paymentSessionModel = await _stripeService.GetPaymentSessionAsync(user.StripeCustomerId, model.StripePriceId);

        return paymentSessionModel;
    }

    public async Task<PaymentSessionModel> GetRenewPaymentSessionAsync()
    {
        var currentSubscription = await _userService.GetSubscriptionAsync();

        if (currentSubscription is null || currentSubscription.IsTrial)
            throw new ServerException("Unable to renew, you have not current subscription");

        var user = await _accountService.GetCurrentUserAsync();

        var paymentSessionModel = await _stripeService.GetPaymentSessionAsync(user.StripeCustomerId, currentSubscription.PriceId);

        return paymentSessionModel;
    }

    public async Task CancelAsync()
    {
        var currentSubscription = await _userService.GetSubscriptionAsync();

        if (currentSubscription is null || currentSubscription.IsTrial)
            throw new ServerException("Unable to cancel, you have no subscriptions");

        await _stripeService.CancelSubscriptionAsync(currentSubscription.Id);
    }

    public async Task ChangeSubscriptionAsync(ChangeSubscriptionModel model)
    {
        var currentSubscription = await _userService.GetSubscriptionAsync();

        if (currentSubscription is null || currentSubscription.IsTrial)
            throw new ServerException("In order to change subscription, you should be subscribed");

        await _stripeService.ChangeSubscriptionAsync(
            currentSubscription.Id,
            model.StripePriceId,
            currentSubscription.SubscriptionItemId
        );
    }

    private string GenerateSubscriptionResponse(bool value)
    {
        var guild = Guid.NewGuid().ToString();
        guild = guild[..^2];
        
        var minValue = value ? 8 : 0;
        var maxValue = value ? int.MaxValue : 8;
        
        var randomValue = _random.Next(minValue, maxValue);
        var res = guild + randomValue.ToString("X2").ToLower();

        return res;
    }
}