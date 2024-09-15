using AutoMapper;
using Stripe;
using StripeIntegration.Shared.Enums;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.BLL.MapperProfiles;

public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Subscription, SubscriptionModel>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            ).ForMember(
                dest => dest.Currency,
                opt => opt.MapFrom(src => src.Items.ToList()[0].Price.Currency)
            ).ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(src => src.Items.ToList()[0].Price.UnitAmountDecimal / 100m)
            ).ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => MapStatus(src.Status))
            ).ForMember(
                dest => dest.PriceId,
                opt => opt.MapFrom(src => src.Items.ToList()[0].Price.Id)
            ).ForMember(
                dest => dest.SubscriptionItemId,
                opt => opt.MapFrom(src => src.Items.ToList()[0].Id)
            ).ForMember(
                dest => dest.ProductId,
                opt => opt.MapFrom(src => src.Items.ToList()[0].Price.ProductId)
            ).ForMember(
                dest => dest.Created,
                opt => opt.MapFrom(src => src.Created)
            ).ForMember(
                dest => dest.CurrentPeriodStart,
                opt => opt.MapFrom(src => src.CurrentPeriodStart)
            ).ForMember(
                dest => dest.CurrentPeriodEnd,
                opt => opt.MapFrom(src => src.CurrentPeriodEnd)
            );
    }

    private static SubscriptionStatus MapStatus(string value) => value switch
    {
        "incomplete" => SubscriptionStatus.Incomplete,
        "incomplete_expired" => SubscriptionStatus.IncompleteExpired,
        "trialing" => SubscriptionStatus.Trialing,
        "active" => SubscriptionStatus.Active,
        "past_due" => SubscriptionStatus.PastDue,
        "canceled" => SubscriptionStatus.Canceled,
        "unpaid" => SubscriptionStatus.Unpaid,
        _ => SubscriptionStatus.Unknown
    };
}

