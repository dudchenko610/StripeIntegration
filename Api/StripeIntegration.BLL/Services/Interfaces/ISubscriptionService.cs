using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.BLL.Services.Interfaces;

public interface ISubscriptionService
{
    Task<string> CheckLicenseKeyAsync(SubscriptionKeyModel model);
    Task<PaymentSessionModel> GetPaymentSessionAsync(GetPaymentSessionModel model);
    Task<PaymentSessionModel> GetRenewPaymentSessionAsync();
    Task ChangeSubscriptionAsync(ChangeSubscriptionModel model);
    Task CancelAsync();
}