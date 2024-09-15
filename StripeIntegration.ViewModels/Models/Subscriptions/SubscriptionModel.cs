using StripeIntegration.Shared.Enums;

namespace StripeIntegration.ViewModels.Models.Subscriptions;

public class SubscriptionModel
{
    public string Id { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsTrial { get; set; }

    public SubscriptionStatus Status { get; set; }

    public string SubscriptionItemId { get; set; } = string.Empty;
    public string PriceId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public DateTime Created { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
}
