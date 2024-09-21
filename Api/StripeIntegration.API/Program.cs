using AspNet.OAuth.Providers.Extensions;
using AspNetCoreRateLimit;
using Stripe;
using StripeIntegration.Shared.Constants;
using StripeIntegration.ViewModels.Options;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddSingleton<IConfiguration>(configuration);

// needed to store rate limit counters and ip rules
services.AddMemoryCache();

// inject counter and rules stores
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

// the IHttpContextAccessor service is not registered by default.
// the clientId/clientIp resolvers use it.
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// configuration (resolvers, counter key builders)
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

services.AddSocialAuthentication()
    .AddGoogle(x =>
    {
        x.ClientId = configuration[$"{Constants.AppSettings.SocialAuthConfiguration}:Google:ClientId"];
        x.ClientSecret = configuration[$"{Constants.AppSettings.SocialAuthConfiguration}:Google:ClientSecret"];
    }).Build();

StripeIntegration.BLL.StartupExtensions.BusinessLogicInitializer(services, configuration, true);

services.Configure<JwtConnectionOptions>(configuration.GetSection(Constants.AppSettings.JwtConfiguration));
services.Configure<EmailConnectionOptions>(configuration.GetSection(Constants.AppSettings.EmailConfiguration));
services.Configure<ClientConnectionOptions>(configuration.GetSection(Constants.AppSettings.ClientConfiguration));
services.Configure<StripeOptions>(configuration.GetSection(Constants.AppSettings.StripeConfiguration));
services.Configure<LicenseKeyOptions>(configuration.GetSection(Constants.AppSettings.LicenseKeyConfiguration));
services.Configure<IpRateLimitOptions>(configuration.GetSection(Constants.AppSettings.IpRateLimitingConfiguration));

StripeConfiguration.ApiKey = configuration[$"{Constants.AppSettings.StripeConfiguration}:{nameof(StripeOptions.SecretKey)}"];

services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpRateLimiting();
app.UseStaticFiles();
app.UseDefaultFiles();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(policyBuilder =>
{
    policyBuilder.WithOrigins(configuration.GetSection(Constants.AppSettings.CorsConfiguration).Get<string[]>() ?? new []{ ""})
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();