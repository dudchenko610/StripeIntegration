using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.BLL.Services.Interfaces;

public interface IUserService
{
    Task<SubscriptionModel?> GetSubscriptionAsync(string userId = "");
    Task<List<ProductModel>> GetProductsAsync();
    Task<SubscriptionModel?> GetSubscriptionByCustomerIdAsync(string stripeCustomerId);
    Task<UserModel> GetUserByIdAsync(string id);
}