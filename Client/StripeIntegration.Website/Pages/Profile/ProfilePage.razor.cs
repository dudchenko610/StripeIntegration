using Microsoft.AspNetCore.Components;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.ViewModels.Models.Subscriptions;
using StripeIntegration.ViewModels.Models.Users;

namespace StripeIntegration.Website.Pages.Profile;

public partial class ProfilePage
{
    [Inject] public required ISubscriptionService SubscriptionService { get; set; }
    [Inject] public required IUserService UserService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly string _applicationsId = $"_id_{Guid.NewGuid()}";

    private UserModel _userModel = new();
    private SubscriptionModel? _subscriptionModel;

    private bool _cancelSubscriptionModalOpened = false;

    private bool _showUserAction = false;
    private ElementReference _showUserActionsElRef;
    
    private bool _showSubscriptionAction = false;
    private ElementReference _showSubscriptionElRef;

    private bool _disabledFields = false;
    private bool _dataIsLoaded = false;
    private bool _applicationsAreLoaded = false;
    private bool _profileUpdating = false;
    private bool _subscriptionChanging = false;

    protected override async Task OnInitializedAsync()
    {
        _applicationsAreLoaded = false;
        StateHasChanged();
        
        _userModel = await UserService.GetAsync() ?? new();
        _subscriptionModel = await SubscriptionService.GetSubscriptionAsync();

        _subscriptionChanging = false;
        _dataIsLoaded = true;
        _applicationsAreLoaded = true;
        
        StateHasChanged();
    }
    
    private async Task OnCancelClickedAsync()
    {
        _subscriptionChanging = true;
        _cancelSubscriptionModalOpened = false;
        StateHasChanged();

        await SubscriptionService.CancelAsync();
        
        await OnInitializedAsync();
    }

    private async Task OnSaveClickedAsync()
    {
        _disabledFields = !_disabledFields;
        _showUserAction = false;
        _profileUpdating = true;
        
        StateHasChanged();

        await UserService.UpdateAsync(new UpdateUserModel { Nickname = _userModel.Nickname });
        _userModel = await UserService.GetAsync() ?? new UserModel();
        _profileUpdating = false;
        
        StateHasChanged();
    }

    private async Task OnSubscriptionDetailsClickedAsync()
    {
        _showSubscriptionAction = !_showSubscriptionAction;
        StateHasChanged();

        if (_showSubscriptionAction)
        {
            await _showSubscriptionElRef.FocusAsync();
        }
    }

    private void OnSubscriptionDetailsFocusOut()
    {
        _showSubscriptionAction = false;
        StateHasChanged();
    }
    
    private async Task OnUserActionsClickedAsync()
    {
        _showUserAction = !_showUserAction;
        StateHasChanged();

        if (_showUserAction)
        {
            await _showUserActionsElRef.FocusAsync();
        }
    }
    
    private void OnUserActionsFocusOut()
    {
        _showUserAction = false;
        StateHasChanged();
    }
}