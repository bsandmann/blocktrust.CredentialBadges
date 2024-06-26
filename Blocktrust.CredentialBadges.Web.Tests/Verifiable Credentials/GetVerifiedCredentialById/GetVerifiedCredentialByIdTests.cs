namespace Blocktrust.CredentialBadges.Web.Tests;

using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;
using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Entities;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public partial class TestSetup
{
    [Fact]
    public async Task GetVerifiedCredentialById_ExistingCredential_ShouldSucceed()
    {
        // Arrange
        var credentialEntity = new VerifiedCredentialEntity
        {
            StoredCredentialId = Guid.NewGuid(),
            Name = "Test Credential",
            Description = "Test Description",
            Image = "https://example.com/image.jpg",
            Credential = "{ \"some\": \"credential data\" }",
            Status = VerifiedCredentialEntity.CredentialStatus.Verified
        };

        _context.Set<VerifiedCredentialEntity>().Add(credentialEntity);
        await _context.SaveChangesAsync();

        var handler = new GetVerifiedCredentialByIdHandler(_context);
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
        result.Value.Status.Should().Be(VerifiedCredential.CredentialStatus.Verified);
    }

    [Fact]
    public async Task GetVerifiedCredentialById_NonExistingCredential_ShouldFail()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var handler = new GetVerifiedCredentialByIdHandler(_context);
        var request = new GetVerifiedCredentialByIdRequest(nonExistingId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeFailure();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be($"Credential with ID {nonExistingId} not found.");
    }
}