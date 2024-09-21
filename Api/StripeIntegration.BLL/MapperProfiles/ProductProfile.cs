using AutoMapper;
using Stripe;
using StripeIntegration.ViewModels.Models.Products;

namespace StripeIntegration.BLL.MapperProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductModel>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            ).ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name)
            ).ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description)
            ).ForMember(
                dest => dest.PriceId,
                opt => opt.MapFrom(src => src.DefaultPrice.Id)
            ).ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(src => src.DefaultPrice.UnitAmountDecimal!.Value / 100m)
            ).ForMember(
                dest => dest.ImgName,
                opt => opt.MapFrom(src => src.Metadata[Shared.Constants.Constants.Stripe.ImgNameKey]));
    }
}