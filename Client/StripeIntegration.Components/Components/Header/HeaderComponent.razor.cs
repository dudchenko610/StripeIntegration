using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Models.Events;
using BlazorComponentHeap.Shared.Models.Math;

namespace StripeIntegration.Components.Components.Header;

public partial class HeaderComponent : IAsyncDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Inject] public required IAuthenticationService AuthenticationService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    [Parameter] public required EventCallback OnBurgerClicked { get; set; }
    
    private bool _showProfileOptions = false;
    private readonly Vec2 _profileOptionsPos = new();
    private readonly NumberFormatInfo _nF = new() { NumberDecimalSeparator = "." };

    private readonly string _profileOptionsId = $"_id_{Guid.NewGuid()}";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private DotNetObjectReference<HeaderComponent> _dotNetRef = null!;

    protected override async Task OnInitializedAsync()
    {
        IJSUtilsService.OnResize += OnResize;
        
        _dotNetRef = DotNetObjectReference.Create(this);
        
        await JsRuntime.InvokeVoidAsync("bchAddDocumentListener", _subscriptionKey, "mousedown", _dotNetRef,
            "OnDocumentMouseDown");
    }
    
    public async ValueTask DisposeAsync()
    {
        await JsRuntime.InvokeVoidAsync("bchRemoveDocumentListener", _subscriptionKey, "mousedown");
        IJSUtilsService.OnResize -= OnResize;
    }

    private Task OnResize()
    {
        if (!_showProfileOptions) return Task.CompletedTask;
        
        _showProfileOptions = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    [JSInvokable]
    public void OnDocumentMouseDown(ExtMouseEventArgs e)
    {
        var coordsHolder = e.PathCoordinates.FirstOrDefault(x => x.Id == _profileOptionsId);
        if (coordsHolder != null) return;

        var showState = _showProfileOptions;
        _showProfileOptions = false;
        
        if (showState) StateHasChanged();
    }
    
    private async Task OnProfileClickedAsync()
    {
        var screenSize = await JsRuntime.InvokeAsync<Vec2>("getScreenBounds");
        _profileOptionsPos.Set(screenSize.X - (screenSize.X < 768 ? 135 : 155), 65);
        
        _showProfileOptions = !_showProfileOptions;
        StateHasChanged();
    }

    private void OnProfileClicked()
    {
        NavigationManager.NavigateTo(Constants.ClientRoutes.Profile);

        _showProfileOptions = false;
        StateHasChanged();
    }

    private async Task OnLogoutClickedAsync()
    {
        await AuthenticationService.LogoutAsync();
        NavigationManager.NavigateTo(Constants.ClientRoutes.Dashboard);
        
        _showProfileOptions = false;
        StateHasChanged();
    }
}
