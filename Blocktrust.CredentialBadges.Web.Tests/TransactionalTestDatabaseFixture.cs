namespace Blocktrust.CredentialBadges.Web.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

public class TransactionalTestDatabaseFixture
{
    /// <summary>
    ///   Replace the connection string with the one appropriate for your environment.
    /// </summary>
    private const string ConnectionString = "Host=10.10.20.103; Database=CredentialBadgesTests; Username=postgres; Password=postgres";

    private readonly ServiceProvider _serviceProvider;

    /// <summary>
    ///   Expose the IServiceScopeFactory so tests can create scopes and use the same Db.
    /// </summary>
    public IServiceScopeFactory ServiceScopeFactory => _serviceProvider.GetRequiredService<IServiceScopeFactory>();

    public TransactionalTestDatabaseFixture()
    {
        // 1. Build our test DI container
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(ConnectionString)
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddConsole();
        });

        _serviceProvider = services.BuildServiceProvider();

        // 2. Ensure a fresh test database
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    /// <summary>
    ///   Called after each test to clean up or save final state if needed.
    ///   Here, you could re-seed or clear the DB. Currently empty for simplicity.
    /// </summary>
    public void Cleanup()
    {
        // No-op or add logic to reset data after each test
    }
}