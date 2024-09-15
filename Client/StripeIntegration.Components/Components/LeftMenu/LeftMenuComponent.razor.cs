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

    private bool _showListComponents = false;
    private List<MenuItemModel> _components = new();
    private bool _isMobile = false;
    private bool _isShown = false;

    protected override void OnInitialized()
    {
        _isMobile = ((IJSInProcessRuntime)JsRuntime).Invoke<bool>("isMobileDevice");
        NavigationManager.LocationChanged += LocationChanged!;

        _components.AddRange(new List<MenuItemModel>
        {
            new () { Name = "Table", Link = Constants.ClientRoutes.TableExample },
            new () { Name = "Select", Link = Constants.ClientRoutes.SelectExample },
            new () { Name = "Tabs", Link = Constants.ClientRoutes.TabsExample },
            new () { Name = "Calendar", Link = Constants.ClientRoutes.CalendarExample },
            new () { Name = "Cropper", Link = Constants.ClientRoutes.CropperExample },
            new () { Name = "Modal", Link = Constants.ClientRoutes.ModalExample },
            new () { Name = "Range", Link = Constants.ClientRoutes.RangeExample },
        });

        _components = _components.OrderBy(x => x.Name).ToList();
    }
    
    public void Dispose()
    {
        NavigationManager.LocationChanged -= LocationChanged!;
    }

    private void OnRedirect(string link)
    {
        switch (link)
        {
            case Constants.ClientRoutes.Components:
            {
                _showListComponents = !_showListComponents;

                if (!_isShown)
                {
                    _isShown = true;
                    _showListComponents = true;
                }
                
                break;
            }
            default:
            {
                if (_isMobile) _isShown = false;
                NavigationManager.NavigateTo(link);
                break;
            }
        }
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