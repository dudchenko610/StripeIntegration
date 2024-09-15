using AutoMapper;
using Microsoft.Extensions.Options;
using Stripe;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;
using StripeIntegration.ViewModels.Models.Users;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Services;

public class UserService : IUserService
{
    private readonly IStripeService _stripeService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IAccountService _accountService;
    private readonly LicenseKeyOptions _licenseKeyOptions;

    public UserService(
        IStripeService stripeService, 
        IMapper mapper, 
        IUserRepository userRepository,
        IAccountService accountService, 
        IOptions<LicenseKeyOptions> licenseKeyOptions)
    {
        _stripeService = stripeService;
        _mapper = mapper;
        _userRepository = userRepository;
        _accountService = accountService;
        _licenseKeyOptions = licenseKeyOptions.Value;
    }

    public async Task<SubscriptionModel?> GetSubscriptionAsync(string userId = "")
    {
        var userModel = await (!string.IsNullOrWhiteSpace(userId) 
            ? GetUserByIdAsync(userId) 
            : _accountService.GetCurrentUserAsync());
        
        var subscriptionModel = await GetSubscriptionByCustomerIdAsync(userModel.StripeCustomerId);

        if (subscriptionModel is null) // subscription is not bought
        {
            var user = await _userRepository.GetByIdAsync(userModel.Id);

            // trial still is active
            if ((DateTime.Now.ToUniversalTime() - user.RegistrationTime).TotalDays <= _licenseKeyOptions.TrialKeyExpirationInDays)
            {
                subscriptionModel = new SubscriptionModel
                {
                    ProductName = "Trial",
                    IsTrial = true,
                    Created = user.RegistrationTime,
                    CurrentPeriodStart = user.RegistrationTime,
                    CurrentPeriodEnd = user.RegistrationTime.AddDays(_licenseKeyOptions.TrialKeyExpirationInDays)
                };
            }
        }

        return subscriptionModel;
    }

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        var stripeProducts = await _stripeService.GetProductsAsync();

        var products = _mapper.Map<List<Product>, List<ProductModel>>(stripeProducts
            .Where(x => x.Metadata[Shared.Constants.Constants.Stripe.AppTypeKey] == "StripeIntegrationApp")
            .ToList()
        );

        return products;
    }

    public async Task<SubscriptionModel?> GetSubscriptionByCustomerIdAsync(string stripeCustomerId)
    {
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(stripeCustomerId);
        var notCanceledSubscriptions = subscriptions.Where(x => x.Status != "canceled").ToList();

        if (notCanceledSubscriptions.Count == 0)
            return null;

        if (notCanceledSubscriptions.Count > 1)
        {
            throw new SystemException("PIZDEC"); // TODO: need to think about, never should happen in theory
        }

        var subscription = await _stripeService.GetSubscriptionAsync(notCanceledSubscriptions[0].Id);
        var subscriptionModel = _mapper.Map<Stripe.Subscription, SubscriptionModel>(subscription);

        var product = await _stripeService.GetProductAsync(subscriptionModel.ProductId);
        subscriptionModel.ProductName = product.Name;

        return subscriptionModel;
    }
    
    public async Task<UserModel> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(new Guid(id));
        var userModel = _mapper.Map<UserModel>(user);

        return userModel;
    }
}