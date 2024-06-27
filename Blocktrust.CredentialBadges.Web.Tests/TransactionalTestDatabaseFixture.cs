namespace Blocktrust.CredentialBadges.Web.Tests;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class TransactionalTestDatabaseFixture
{
    /// <summary>
    /// Replace the connection string with the one that is appropriate for your local test environment.
    /// </summary>
    
    // Bjorn Home 
    // private const string ConnectionString = @"Host=192.168.178.163; Database=CredentialBadgesTests; Username=postgres; Password=password";
    // Bjoern Office
    private const string ConnectionString = @"Host=10.10.20.103; Database=CredentialBadgesTests; Username=postgres; Password=postgres";

    public ApplicationDbContext CreateContext()
        => new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .EnableSensitiveDataLogging(true)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options);

    public TransactionalTestDatabaseFixture()
    {
        using var context = CreateContext();
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Cleanup();
    }

    public void Cleanup()
    {
        // using var context = CreateContext();
        // context.SaveChanges();
    }
}