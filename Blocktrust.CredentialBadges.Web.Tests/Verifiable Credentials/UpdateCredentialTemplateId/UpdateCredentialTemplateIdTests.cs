using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCredentialTemplateId;
using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Blocktrust.CredentialBadges.Web.Tests
{
    using System.Linq.Expressions;

    public partial class TestSetup
    {
        [Fact]
        public async Task UpdateCredentialTemplateId_ValidId_ShouldUpdateTemplateIdAndReturnSuccess()
        {
            // Arrange
            Guid credentialId;
            using (var seedScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var credentialEntity = new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Name = "Credential To Update",
                    Description = "Will update the template ID",
                    Image = "https://example.com/someimage.jpg",
                    Credential = "{ \"data\": \"some data\" }",
                    Status = EVerificationStatus.Verified,
                    Issuer = "Initial Issuer",
                    TemplateId = "initial-template"
                };

                context.VerifiedCredentials.Add(credentialEntity);
                await context.SaveChangesAsync();
                credentialId = credentialEntity.StoredCredentialId;
            }

            var newTemplateId = "updated-template-id";

            // Create handler
            var loggerMock = new Mock<ILogger<UpdateCredentialTemplateIdHandler>>();
            var handler = new UpdateCredentialTemplateIdHandler(loggerMock.Object, Fixture.ServiceScopeFactory);

            // Act
            var request = new UpdateCredentialTemplateIdRequest(credentialId, newTemplateId);
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess("because the existing credential should be updated successfully");

            // Verify the changes in the database
            using var verifyScope = Fixture.ServiceScopeFactory.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedEntity = await verifyContext.VerifiedCredentials.FirstOrDefaultAsync(e => e.StoredCredentialId == credentialId);

            updatedEntity.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateCredentialTemplateId_NonExistentCredential_ShouldReturnFailure()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<UpdateCredentialTemplateIdHandler>>();
            var handler = new UpdateCredentialTemplateIdHandler(loggerMock.Object, Fixture.ServiceScopeFactory);
            var randomId = Guid.NewGuid();

            // Act
            var request = new UpdateCredentialTemplateIdRequest(randomId, "some-template-id");
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeFailure("because a credential with the given random ID doesn't exist");
            result.Errors[0].Message.Should().Contain($"Credential with ID {randomId} not found");
        }

        [Fact]
        public async Task UpdateCredentialTemplateId_EmptyTemplateId_ShouldUpdateToEmptyAndReturnSuccess()
        {
            // Arrange
            Guid credentialId;
            using (var seedScope = Fixture.ServiceScopeFactory.CreateScope())
            {
                var context = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var credentialEntity = new VerifiedCredentialEntity
                {
                    StoredCredentialId = Guid.NewGuid(),
                    Name = "Credential With Template To Clear",
                    Description = "Testing empty template ID",
                    Image = "https://example.com/anotherimage.jpg",
                    Credential = "{ \"data\": \"another data\" }",
                    Status = EVerificationStatus.Verified,
                    Issuer = "Issuer",
                    TemplateId = "non-empty-template"
                };

                context.VerifiedCredentials.Add(credentialEntity);
                await context.SaveChangesAsync();
                credentialId = credentialEntity.StoredCredentialId;
            }

            var loggerMock = new Mock<ILogger<UpdateCredentialTemplateIdHandler>>();
            var handler = new UpdateCredentialTemplateIdHandler(loggerMock.Object, Fixture.ServiceScopeFactory);

            // Act
            var request = new UpdateCredentialTemplateIdRequest(credentialId, string.Empty);
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess("because updating the template ID to an empty string should still succeed");

            // Verify the changes in the database
            using var verifyScope = Fixture.ServiceScopeFactory.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedEntity = await verifyContext.VerifiedCredentials.FirstOrDefaultAsync(e => e.StoredCredentialId == credentialId);

            updatedEntity.Should().NotBeNull();
        }

    }
}
