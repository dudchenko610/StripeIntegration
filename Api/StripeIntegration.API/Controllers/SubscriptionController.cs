using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.API.Controllers;

[ApiController]
[Route(Constants.ApiRoutes.SUBSCRIPTION)]
public class SubscriptionController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;

    public SubscriptionController(
        ISubscriptionService subscriptionService, 
        IUserService userService)
    {
        _subscriptionService = subscriptionService;
        _userService = userService;
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.CHECK_LICENSE_KEY)]
    public async Task<IActionResult> CheckLicenseKey(SubscriptionKeyModel model)
    {
        var result = await _subscriptionService.CheckLicenseKeyAsync(model);
        return Ok(result);
    }

    [HttpGet]
    [Route(Constants.ApiRoutes.GET_PRODUCTS)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _userService.GetProductsAsync();

        return Ok(products);
    }

    [HttpGet]
    [Route(Constants.ApiRoutes.GET_SUBSCRIPTION)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> GetSubscription()
    {
        var subscription = await _userService.GetSubscriptionAsync();

        return Ok(subscription);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.GET_PAYMENT_SESSION)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> GetPaymentSession(GetPaymentSessionModel model)
    {
        var paymentSessionModel = await _subscriptionService.GetPaymentSessionAsync(model);

        return Ok(paymentSessionModel);
    }

    [HttpGet]
    [Route(Constants.ApiRoutes.GET_RENEW_PAYMENT_SESSION)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> GetRenewPaymentSession()
    {
        var paymentSessionModel = await _subscriptionService.GetRenewPaymentSessionAsync();

        return Ok(paymentSessionModel);
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.CHANGE_SUBSCRIPTION)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> ChangeSubscription(ChangeSubscriptionModel model)
    {
        await _subscriptionService.ChangeSubscriptionAsync(model);

        return Ok(new { });
    }

    [HttpPost]
    [Route(Constants.ApiRoutes.CANCEL_SUBSCRIPTION)]
    [Authorize(AuthenticationSchemes = Constants.Token.BEARER)]
    public async Task<IActionResult> CancelSubscription()
    {
        await _subscriptionService.CancelAsync();

        return Ok(new { });
    }
}
