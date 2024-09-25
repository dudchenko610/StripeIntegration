using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using StripeIntegration.Shared.Constants;

namespace StripeIntegration.Components.Components.LeftMenu;

public partial class LeftMenuComponent : IDisposable
{
    private class MenuItemModel
    {
        public required string Name { get; set; }
        public required string Link { get; set; }
    }
    
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    private bool _isMobile = false;
    private bool _isShown = false;

    protected override void OnInitialized()
    {
        _isMobile = ((IJSInProcessRuntime)JsRuntime).Invoke<bool>("isMobileDevice");
        NavigationManager.LocationChanged += LocationChanged!;
    }
    
    public void Dispose()
    {
        NavigationManager.LocationChanged -= LocationChanged!;
    }

    private void OnRedirect(string link)
    {
        if (_isMobile) _isShown = false;
        NavigationManager.NavigateTo(link);
    }

    private void OnComponentClicked(string link)
    {
        if (_isMobile) _isShown = false;
        NavigationManager.NavigateTo(link);
    }

    private bool IsExampleRefActive(string href)
    {
        var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();
        return relativePath.Contains("components") && href.Contains(relativePath);
    }

    private void LocationChanged(object sender, LocationChangedEventArgs e) => StateHasChanged();
    
    public void Show(bool isShown)
    {
        _isShown = isShown;
        StateHasChanged();
    }
    
    public void SwapShow()
    {
        _isShown = !_isShown;
        StateHasChanged();
    }
}