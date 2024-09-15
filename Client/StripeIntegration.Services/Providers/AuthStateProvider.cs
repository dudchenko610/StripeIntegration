using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using StripeIntegration.Service.Helpers;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Shared.Constants;

namespace StripeIntegration.Service.Providers;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    public AuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(Constants.Auth.AccessToken);

        //Console.WriteLine("token = " + token);

        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }

        try
        {
            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));

            // Console.WriteLine("as = " + authState.User.Identity.IsAuthenticated);

            return authState;
        }
        catch
        {
            return _anonymous;
        }

    }

    public void NotifyUserAuthentication(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }
}
