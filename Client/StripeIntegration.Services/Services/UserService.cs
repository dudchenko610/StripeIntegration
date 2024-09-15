using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.ViewModels.Models;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.Service.Services;

public class UserService(IHttpService httpService) : IUserService
{
    public async Task<UserModel?> GetAsync()
    {
        var response = await httpService.GetAsync<UserModel>("api/account/get-user");
        return response.Result;
    }

    public async Task<bool> UpdateAsync(UpdateUserModel model)
    {
        var response = await httpService.PostAsync<ApiResponse<object>, UpdateUserModel>("api/account/update-user", model);
        return response.Exception is null;
    }
}