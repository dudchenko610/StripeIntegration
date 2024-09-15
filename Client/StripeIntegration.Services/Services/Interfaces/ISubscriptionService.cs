using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.Service.Services.Interfaces;

public interface ISubscriptionService
{
    Task<List<ProductModel>> GetProductsAsync(bool showError = true);
    Task<PaymentSessionModel?> GetPaymentSessionAsync(GetPaymentSessionModel model);
    Task<SubscriptionModel?> GetSubscriptionAsync(bool showError = true);
    Task<PaymentSessionModel?> GetRenewPaymentSessionAsync();

    Task ChangeSubscriptionAsync(ChangeSubscriptionModel model);
    Task CancelAsync();
}