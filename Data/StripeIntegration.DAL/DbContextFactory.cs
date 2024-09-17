using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StripeIntegration.Shared.Constants;

namespace StripeIntegration.DAL;

public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.FullName ?? string.Empty)
            .AddJsonFile("Api/StripeIntegration.API/appsettings.Development.json").Build();

        var builder = new DbContextOptionsBuilder<DatabaseContext>();
        var connectionString = configuration.GetConnectionString(Constants.AppSettings.SqlServerConnection);
        
        builder.UseNpgsql(connectionString);

        return new DatabaseContext(builder.Options);
    }
}