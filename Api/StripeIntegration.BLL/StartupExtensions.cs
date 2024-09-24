using System.Text;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StripeIntegration.BLL.MapperProfiles;
using StripeIntegration.BLL.Providers;
using StripeIntegration.BLL.Services;
using StripeIntegration.BLL.Services.Interfaces;
using StripeIntegration.Shared.Constants;

namespace StripeIntegration.BLL;

public static class StartupExtensions
{
    public static void BusinessLogicInitializer(this IServiceCollection service, IConfiguration configuration, bool migrateDb = false)
    {
        DAL.StartupExtensions.DataAccessInitializer(service, configuration, migrateDb);

        #region Service
        service.AddScoped<IAccountService, AccountService>();
        service.AddScoped<ISubscriptionService, SubscriptionService>();
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IStripeService, StripeService>();
        
        service.AddScoped<Stripe.EventService>();
        service.AddScoped<Stripe.RefundService>();
        service.AddScoped<Stripe.InvoiceService>();
        service.AddScoped<Stripe.ProductService>();
        service.AddScoped<Stripe.Checkout.SessionService>();
        service.AddScoped<Stripe.CustomerService>();
        service.AddScoped<Stripe.SubscriptionService>();
        service.AddScoped<Stripe.PriceService>();
        #endregion
        
        #region Provider
        service.AddScoped<JwtProvider>();
        service.AddScoped<EmailProvider>();
        #endregion
        
        service.AddHttpContextAccessor();
        
        var mapperConfig = new MapperConfiguration(config =>
        {
            config.Internal().MethodMappingEnabled = false;
            
            config.AddProfile(new UserProfile());
            config.AddProfile(new ProductProfile());
            config.AddProfile(new SubscriptionProfile());
        });
        
        IMapper mapper = mapperConfig.CreateMapper();
        service.AddSingleton(mapper);

        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(0),
                    ValidateIssuer = true,
                    ValidIssuer = configuration[$"{Constants.AppSettings.JwtConfiguration}:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration[$"{Constants.AppSettings.JwtConfiguration}:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration[$"{Constants.AppSettings.JwtConfiguration}:SecretKey"] ?? string.Empty)),
                    ValidateIssuerSigningKey = true,
                };
            });
    }
}
