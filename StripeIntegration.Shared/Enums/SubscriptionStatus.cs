using System.ComponentModel.DataAnnotations;

namespace StripeIntegration.Shared.Enums;

public enum SubscriptionStatus
{
    [Display(Name = "incomplete")]
    Incomplete,

    [Display(Name = "incomplete_expired")]
    IncompleteExpired,

    [Display(Name = "trialing")]
    Trialing,

    [Display(Name = "active")]
    Active,

    [Display(Name = "past_due")]
    PastDue,

    [Display(Name = "canceled")]
    Canceled,

    [Display(Name = "unpaid")]
    Unpaid,

    [Display(Name = "unknown")]
    Unknown
}