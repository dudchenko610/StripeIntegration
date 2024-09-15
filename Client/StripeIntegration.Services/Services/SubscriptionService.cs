using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.Service.Services;

public class SubscriptionService(IHttpService httpService) : ISubscriptionService
{
    public async Task<PaymentSessionModel?> GetPaymentSessionAsync(GetPaymentSessionModel model)
    {
        var paymentSessionModel = await httpService
            .PostAsync<PaymentSessionModel, GetPaymentSessionModel>("api/subscription/get-payment-session", model);

        return paymentSessionModel.Result;
    }

    public async Task<List<ProductModel>> GetProductsAsync(bool showError = true)
    {
        var products = await httpService
            .GetAsync<List<ProductModel>>("api/subscription/get-products", showError);

        return products.Result ?? new();
    }

    public async Task<SubscriptionModel?> GetSubscriptionAsync(bool showError = true)
    {
        var subscription = await httpService
            .GetAsync<SubscriptionModel>("api/subscription/get-subscription", showError);

        return subscription.Result;
    }

    public async Task CancelAsync()
    {
        await httpService.PostAsync<object>("api/subscription/cancel-subscription");
    }

    public async Task ChangeSubscriptionAsync(ChangeSubscriptionModel model)
    {
        await httpService.PostAsync<object, ChangeSubscriptionModel>("api/subscription/change-subscription", model);
    }

    public async Task<PaymentSessionModel?> GetRenewPaymentSessionAsync()
    {
        var paymentSessionModel = await httpService
            .PostAsync<PaymentSessionModel, object>("api/subscription/get-renew-payment-session", new());

        return paymentSessionModel.Result;
    }
}