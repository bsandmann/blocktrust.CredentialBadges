using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCache;
using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;
using Blocktrust.CredentialBadges.Web.Domain;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blocktrust.CredentialBadges.Web.Tests
{
    public partial class TestSetup
    {
        [Fact]
        public async Task UpdateCache_EmptyDatabase_ShouldReturnSuccess()
        {
            // Arrange: Ensure the database has no VerifiedCredentials
            using (var scope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var allCredentials = context.VerifiedCredentials.ToList();
                context.VerifiedCredentials.RemoveRange(allCredentials);
                await context.SaveChangesAsync();
            }

            // Create a new handler with an empty or fresh cache
            var cache = new CachingService();
            var handler = new UpdateCacheHandler(Fixture.ServiceScopeFactory, cache);

            // Act
            var request = new UpdateCacheRequest(); // Uses default batchSize=500
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess("because an empty database should not cause an error");
            // Verify that nothing was added to the cache
            // LazyCache doesn't expose a direct "count" method, but typically you'd check
            // for presence of keys or mock the cache if you need to verify calls.
        }

        [Fact]
        public async Task UpdateCache_SingleBatch_AllRecordsCached()
        {
            // Arrange
            // Seed database with a few credentials (fewer than batchSize)
            var seedCredentials = new List<VerifiedCredentialEntity>
            {
                new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Credential = "testCredential",
                    Issuer = "Test Issuer",
                    Status = EVerificationStatus.Verified
                },
                new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Credential = "testCredential",
                    Issuer = "Test Issuer",
                    Status = EVerificationStatus.Verified
                }
            };

            using (var scope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.VerifiedCredentials.RemoveRange(context.VerifiedCredentials); // Clear first
                context.VerifiedCredentials.AddRange(seedCredentials);
                await context.SaveChangesAsync();
            }

            // Use a cache we can later examine
            var cache = new CachingService();
            var handler = new UpdateCacheHandler(Fixture.ServiceScopeFactory, cache);

            // Act: Use a larger batch size so that all records are processed in one go
            var request = new UpdateCacheRequest(batchSize: 100);
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess("because all credentials should be processed in one single batch");

            // Verify entries in the cache
            foreach (var cred in seedCredentials)
            {
                var cacheKey = $"GetVerifiedCredentialById_{cred.StoredCredentialId}";
                var cachedValue = cache.Get<VerifiedCredential>(cacheKey);

                cachedValue.Should().NotBeNull($"Credential {cred.StoredCredentialId} should have been cached");
            }
        }

        [Fact]
        public async Task UpdateCache_MultipleBatches_AllRecordsCached()
        {
            // Arrange
            // Seed database with more credentials than the batch size
            var totalCredentials = 10;
            var batchSize = 3;

            var seedCredentials = Enumerable.Range(1, totalCredentials)
                .Select(i => new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Credential = "testCredential",
                    Issuer = "Test Issuer",
                    Status = EVerificationStatus.Verified
                })
                .ToList();

            using (var scope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.VerifiedCredentials.RemoveRange(context.VerifiedCredentials); // Clear existing
                context.VerifiedCredentials.AddRange(seedCredentials);
                await context.SaveChangesAsync();
            }

            var cache = new CachingService();
            var handler = new UpdateCacheHandler(Fixture.ServiceScopeFactory, cache);

            // Act
            var request = new UpdateCacheRequest(batchSize);
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess("because all credentials should be processed across multiple batches");

            // Verify every credential is in the cache
            foreach (var cred in seedCredentials)
            {
                var cacheKey = $"GetVerifiedCredentialById_{cred.StoredCredentialId}";
                var cachedValue = cache.Get<VerifiedCredential>(cacheKey);

                cachedValue.Should().NotBeNull($"Credential {cred.StoredCredentialId} should be in the cache");
            }
        }
    }
}
