using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WatchCatalogAPI.Data;

public sealed class WatchCatalogDbContextFactory : IDesignTimeDbContextFactory<WatchCatalogDbContext>
{
    public WatchCatalogDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Production";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("WatchCatalog");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The WatchCatalog SQL connection string is missing. Set ConnectionStrings__WatchCatalog.");
        }

        var options = new DbContextOptionsBuilder<WatchCatalogDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new WatchCatalogDbContext(options);
    }
}
