namespace Blocktrust.CredentialBadges.Web.Tests;

using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public partial class TestSetup
{
    [Fact]
    public async Task GetVerifiedCredentialById_ExistingCredential_ShouldSucceed()
    {
        // Arrange
        using (var seedScope = Fixture.ServiceScopeFactory.CreateScope())
        {
            var context = ServiceProviderServiceExtensions.GetRequiredService<ApplicationDbContext>(seedScope.ServiceProvider);
            var credentialEntity = new VerifiedCredentialEntity
            {
                StoredCredentialId = Guid.NewGuid(),
                Name = "Test Credential",
                Description = "Test Description",
                Image = "https://example.com/image.jpg",
                Credential = "{ \"some\": \"credential data\" }",
                Status = EVerificationStatus.Verified,
                Issuer = "Test Issuer"
            };

            context.VerifiedCredentials.Add(credentialEntity);
            await context.SaveChangesAsync();

            // Construct the handler AFTER data is seeded
            var handler = new GetVerifiedCredentialByIdHandler(Fixture.ServiceScopeFactory, new CachingService());
            var request = new GetVerifiedCredentialByIdRequest(credentialEntity.StoredCredentialId);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Test Credential");
            result.Value.Description.Should().Be("Test Description");
            result.Value.Image.Should().Be("https://example.com/image.jpg");
            result.Value.Credential.Should().Be("{ \"some\": \"credential data\" }");
            result.Value.Status.Should().Be(EVerificationStatus.Verified);
        }
    }
}