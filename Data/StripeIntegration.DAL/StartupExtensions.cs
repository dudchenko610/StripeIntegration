using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StripeIntegration.DAL.Repositories;
using StripeIntegration.DAL.Repositories.Interfaces;
using StripeIntegration.Entities.Entities;
using StripeIntegration.Shared.Constants;

namespace StripeIntegration.DAL;

public static class StartupExtensions
{
    public static void DataAccessInitializer(this IServiceCollection service, IConfiguration configuration, bool migrateDb = false)
    {
        var sqlConnectionString = configuration.GetConnectionString(Constants.AppSettings.SqlServerConnection);
        
        service.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(sqlConnectionString, x => x.MigrationsAssembly("StripeIntegration.DAL")));

        service.AddIdentity<User, IdentityRole<Guid>>(opts =>
            {
                opts.Password.RequiredLength = 12;                   // Minimum length requirement
                opts.Password.RequireNonAlphanumeric = true;         // Require at least one special character
                opts.Password.RequireLowercase = true;               // Require at least one lowercase letter
                opts.Password.RequireUppercase = true;               // Require at least one uppercase letter
                opts.Password.RequireDigit = true;                   // Require at least one digit

                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = null!;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

        service.AddTransient<IUserRepository, UserRepository>();

        if (!migrateDb) return;
        
        using var context = service.BuildServiceProvider().GetService<DatabaseContext>();
        context?.Database.Migrate();
    }
}
