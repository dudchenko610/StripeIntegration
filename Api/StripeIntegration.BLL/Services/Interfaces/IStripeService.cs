using Stripe;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.BLL.Services.Interfaces;

public interface IStripeService
{
    Task<StripeList<Product>> GetProductsAsync();
    Task<Product?> GetProductAsync(string productId);
    Task<Product?> GetProductByPriceIdAsync(string priceId);

    Task<string> CreateCustomerAsync(string email);
    Task<PaymentSessionModel> GetPaymentSessionAsync(string customerId, string priceId);
    Task<List<Subscription>> GetCustomerSubscriptionsAsync(string customerId);
    Task<Subscription> GetSubscriptionAsync(string subscriptionId);

    Task CancelSubscriptionAsync(string subscriptionId);
    Task ChangeSubscriptionAsync(string subscriptionId, string newPriceId, string subscriptionItemId);
}