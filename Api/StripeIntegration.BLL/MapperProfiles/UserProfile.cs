using AutoMapper;
using StripeIntegration.Entities.Entities;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.BLL.MapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserModel, User>();
        CreateMap<User, UserModel>();
    }
}