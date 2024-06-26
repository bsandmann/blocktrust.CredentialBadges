namespace Blocktrust.CredentialBadges.Web.Tests;

using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Entities;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public partial class TestSetup
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoreVerifiedCredentialHandler>>();

        var request = new StoreVerifiedCredentialRequest()
        {
            Name = "Test Credential",
            Description = "Test Description",
            Image = "https://example.com/image.jpg",
            Credential = "{ \"some\": \"credential data\" }",
            Status = VerifiedCredential.CredentialStatus.Verified
        };

        var handler = new StoreVerifiedCredentialHandler(_context, loggerMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Test Credential");
        result.Value.Description.Should().Be("Test Description");
        result.Value.Image.Should().Be("https://example.com/image.jpg");
        result.Value.Credential.Should().Be("{ \"some\": \"credential data\" }");
        result.Value.Status.Should().Be(VerifiedCredential.CredentialStatus.Verified);

        // Verify the credential was actually added to the database
        var credentialInDb = await _context.Set<VerifiedCredentialEntity>()
            .FirstOrDefaultAsync(c => c.Name == "Test Credential");
        credentialInDb.Should().NotBeNull();
        credentialInDb!.Description.Should().Be("Test Description");
        credentialInDb.Image.Should().Be("https://example.com/image.jpg");
        credentialInDb.Credential.Should().Be("{ \"some\": \"credential data\" }");
        credentialInDb.Status.Should().Be(VerifiedCredentialEntity.CredentialStatus.Verified);
    }
}