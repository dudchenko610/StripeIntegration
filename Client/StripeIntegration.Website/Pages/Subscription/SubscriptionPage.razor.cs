using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.Website.Pages.Subscription;

public partial class SubscriptionPage
{
    [Inject] public required ISubscriptionService SubscriptionService { get; set; }

    private List<ProductModel> _products = new();
    private SubscriptionModel? _currentSubscription;
    private bool _isLoading = true;
    private bool _buttonLoader;

    protected override Task OnInitializedAsync()
    {
        return UpdateAsync();
    }

    private async Task UpdateAsync()
    {
        _products = await SubscriptionService.GetProductsAsync(false) ?? new();
        _products = _products.OrderBy(x => x.Price).ToList();
        _currentSubscription = await SubscriptionService.GetSubscriptionAsync(false);
        
        _currentSubscription = _currentSubscription is not null && _currentSubscription.IsTrial
            ? null
            : _currentSubscription;
        
        _isLoading = false;
        _buttonLoader = false;
        
        StateHasChanged();
    }

    private void OnButtonLoaderChanged(bool buttonLoader)
    {
        _buttonLoader = buttonLoader;
        StateHasChanged();
    }
}
