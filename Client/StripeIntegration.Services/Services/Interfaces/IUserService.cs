using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.Service.Services.Interfaces;

public interface IUserService
{
    Task<UserModel?> GetAsync();
    Task<bool> UpdateAsync(UpdateUserModel model);
}