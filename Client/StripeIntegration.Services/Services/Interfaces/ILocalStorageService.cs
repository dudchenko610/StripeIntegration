namespace StripeIntegration.Service.Services.Interfaces;

public interface ILocalStorageService
{
    Task<T> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    T GetItem<T>(string key);
    void SetItem<T>(string key, T value);
}