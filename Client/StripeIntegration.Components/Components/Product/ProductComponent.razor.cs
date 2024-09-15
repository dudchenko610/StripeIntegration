using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using StripeIntegration.ViewModels.Models.Products;
using StripeIntegration.ViewModels.Models.Subscriptions;

namespace StripeIntegration.Components.Components.Product;

public partial class ProductComponent
{
    [Inject] public required ISubscriptionService SubscriptionService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    [Parameter] public SubscriptionModel? CurrentSubscription { get; set; }
    [Parameter] public required ProductModel Product { get; set; }
    [Parameter] public required EventCallback OnUpdate { get; set; }
    [Parameter] public required bool ButtonLoader { get; set; }
    [Parameter] public required EventCallback<bool> OnButtonLoaderChanged { get; set; }

    private bool _cancelSubscription;
    private bool _changeSubscription;
    private async Task OnSubscribeClickedAsync()
    {
        await OnButtonLoaderChanged.InvokeAsync(true);
        var paymentSessionModel = await SubscriptionService.GetPaymentSessionAsync(
            new GetPaymentSessionModel
            {
                StripePriceId = Product.PriceId ?? string.Empty
            }
        ) ?? new();

        NavigationManager.NavigateTo(paymentSessionModel == null! || string.IsNullOrWhiteSpace(paymentSessionModel.Url)
            ? Constants.ClientRoutes.SignIn
            : paymentSessionModel.Url);
    }

    private async Task OnChangeSubscriptionAsync()
    {
        await OnButtonLoaderChanged.InvokeAsync(true);
        await SubscriptionService.ChangeSubscriptionAsync(new ChangeSubscriptionModel
        {
            StripePriceId = Product.PriceId ?? string.Empty
        });
        
        await OnUpdate.InvokeAsync();
    }

    private async Task OnCancelAsync()
    {
        _cancelSubscription = false;
        StateHasChanged();
        
        await OnButtonLoaderChanged.InvokeAsync(true);
        await SubscriptionService.CancelAsync();
        await OnUpdate.InvokeAsync();
    }
    
    private async Task OnRenewClickedAsync()
    {
        var paymentSessionModel = await SubscriptionService.GetRenewPaymentSessionAsync() ?? new();
        NavigationManager.NavigateTo(paymentSessionModel.Url);
    }

    private async Task ChangeAsync()
    {
        _changeSubscription = false;
        StateHasChanged();
        
        await OnChangeSubscriptionAsync();
    }
}