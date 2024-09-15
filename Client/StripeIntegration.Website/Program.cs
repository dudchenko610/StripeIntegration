using BlazorComponentHeap.Components.Modal.Root;
using BlazorComponentHeap.Core.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StripeIntegration.Service.Providers;
using StripeIntegration.Service.Services;
using StripeIntegration.Service.Services.Interfaces;
using StripeIntegration.Website;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
    
builder.RootComponents.Add<BCHRootModal>("body::after");

services.AddAuthorizationCore();

services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API:BaseUri"]!) });

services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

services.AddScoped<ILocalStorageService, LocalStorageService>();
services.AddScoped<IHttpService, HttpService>();
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IErrorService, ErrorService>();
services.AddScoped<ISubscriptionService, SubscriptionService>();
services.AddScoped<IUserService, UserService>();

services.AddBCHComponents("subscription_key"); // key should be passed here somehow
// services.Configure<SocialCallbackOption>(configuration.GetSection("SocialCallbacks"));

await builder.Build().RunAsync();