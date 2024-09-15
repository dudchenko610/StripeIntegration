using Microsoft.JSInterop;
using StripeIntegration.Service.Services.Interfaces;

namespace StripeIntegration.Service.Services;

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IJSInProcessRuntime _jsInProcessRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _jsInProcessRuntime = (IJSInProcessRuntime) jsRuntime;
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        return await _jsRuntime.InvokeAsync<T>("window.localStorage.getItem", key);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("window.localStorage.setItem", key, value);
    }

    public T GetItem<T>(string key)
    {
        return _jsInProcessRuntime.Invoke<T>("window.localStorage.getItem", key);
    }

    public void SetItem<T>(string key, T value)
    {
        _jsInProcessRuntime.InvokeVoid("window.localStorage.setItem", key, value);
    }
}