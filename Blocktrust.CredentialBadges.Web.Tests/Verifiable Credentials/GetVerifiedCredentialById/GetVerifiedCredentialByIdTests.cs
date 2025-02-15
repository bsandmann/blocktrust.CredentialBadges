using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blocktrust.CredentialBadges.Web.Tests
{
    public partial class TestSetup
    {
        [Fact]
        public async Task GetVerifiedCredentialById_NonExistentCredential_ShouldReturnFailure()
        {
            // Arrange
            var handler = new GetVerifiedCredentialByIdHandler(Fixture.ServiceScopeFactory, new CachingService());
            var randomId = Guid.NewGuid(); 
            var request = new GetVerifiedCredentialByIdRequest(randomId);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeFailure();
            // The error message should indicate that the credential was not found
            result.Errors[0].Message.Should().Contain($"Credential with ID {randomId} not found");
        }

        [Fact]
        public async Task GetVerifiedCredentialById_SkipCacheTrue_ShouldFetchUpdatedEntity()
        {
            // Arrange
            Guid credentialId;
            using (var seedScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var credentialEntity = new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Name = "Initial Name",
                    Description = "Initial Description",
                    Image = "https://example.com/initial.jpg",
                    Credential = "{ \"some\": \"initial\" }",
                    Status = EVerificationStatus.Verified,
                    Issuer = "Initial Issuer"
                };

                context.VerifiedCredentials.Add(credentialEntity);
                await context.SaveChangesAsync();
                credentialId = credentialEntity.StoredCredentialId;
            }

            // First retrieval: skipCache = true, should fetch from DB
            var handler = new GetVerifiedCredentialByIdHandler(Fixture.ServiceScopeFactory, new CachingService());
            var request = new GetVerifiedCredentialByIdRequest(credentialId, skipCache: true);
            var firstResult = await handler.Handle(request, CancellationToken.None);

            firstResult.Should().BeSuccess();
            firstResult.Value.Name.Should().Be("Initial Name");

            // Now, update the credential in the DB
            using (var updateScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = updateScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var entity = await context.VerifiedCredentials.FirstAsync(c => c.StoredCredentialId == credentialId);
                entity.Name = "Updated Name";
                context.VerifiedCredentials.Update(entity);
                await context.SaveChangesAsync();
            }

            // Second retrieval: skipCache = true again, should fetch the *updated* record from DB
            var secondResult = await handler.Handle(request, CancellationToken.None);
            secondResult.Should().BeSuccess();
            secondResult.Value.Name.Should().Be("Updated Name",
                "because skipCache = true forces the handler to bypass the cache and fetch from DB");
        }

        [Fact]
        public async Task GetVerifiedCredentialById_CachedResult_ShouldReturnStaleDataIfDbIsUpdated()
        {
            // Arrange
            Guid credentialId;
            using (var seedScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var credentialEntity = new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Name = "Cache Test Name",
                    Description = "Cache Test Description",
                    Image = "https://example.com/cache.jpg",
                    Credential = "{ \"cache\": \"test\" }",
                    Status = EVerificationStatus.Verified,
                    Issuer = "Cache Issuer"
                };

                context.VerifiedCredentials.Add(credentialEntity);
                await context.SaveChangesAsync();
                credentialId = credentialEntity.StoredCredentialId;
            }

            // First retrieval: skipCache = false, should store result in cache
            var cache = new CachingService();
            var handler = new GetVerifiedCredentialByIdHandler(Fixture.ServiceScopeFactory, cache);
            var request = new GetVerifiedCredentialByIdRequest(credentialId, skipCache: false);
            var firstResult = await handler.Handle(request, CancellationToken.None);

            firstResult.Should().BeSuccess();
            firstResult.Value.Name.Should().Be("Cache Test Name");

            // Now, update the credential in the DB
            using (var updateScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = updateScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var entity = await context.VerifiedCredentials.FirstAsync(c => c.StoredCredentialId == credentialId);
                entity.Name = "Updated DB Name";
                context.VerifiedCredentials.Update(entity);
                await context.SaveChangesAsync();
            }

            // Second retrieval: skipCache = false, should return the *cached* record, not the updated one
            var secondResult = await handler.Handle(request, CancellationToken.None);
            secondResult.Should().BeSuccess();
            secondResult.Value.Name.Should().Be("Cache Test Name",
                "because skipCache = false reuses the cached version rather than fetching the updated record from DB");
        }
    }
}
