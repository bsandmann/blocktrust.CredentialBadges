using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;
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
    public partial class TestSetup
    {
        [Fact]
        public async Task Handle_WithValidRequest_ShouldSucceed()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StoreVerifiedCredentialHandler>>();
            var request = new StoreVerifiedCredentialRequest
            {
                Image = "https://example.com/image.jpg",
                Credential = "{ \"some\": \"credential data\" }",
                Status = EVerificationStatus.Verified,
                Issuer = "Test Issuer"
            };
            var handler = new StoreVerifiedCredentialHandler(loggerMock.Object, Fixture.ServiceScopeFactory);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess();
            result.Value.Should().NotBeNull();
            result.Value.Image.Should().Be("https://example.com/image.jpg");
            result.Value.Credential.Should().Be("{ \"some\": \"credential data\" }");
            result.Value.Status.Should().Be(EVerificationStatus.Verified);

            // Verify the credential was actually added to the database
            using var scope = Fixture.ServiceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var credentialInDb = await context.Set<VerifiedCredentialEntity>()
                .FirstOrDefaultAsync(c => c.Image == "https://example.com/image.jpg");

            credentialInDb.Should().NotBeNull();
            credentialInDb.Image.Should().Be("https://example.com/image.jpg");
            credentialInDb.Credential.Should().Be("{ \"some\": \"credential data\" }");
            credentialInDb.Status.Should().Be(EVerificationStatus.Verified);
        }

        [Fact]
        public async Task Handle_WithNoImage_ShouldSetNoImageTemplateId()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StoreVerifiedCredentialHandler>>();
            var request = new StoreVerifiedCredentialRequest
            {
                Image = "",  // no image
                Credential = "{ \"some\": \"credential data\" }",
                Status = EVerificationStatus.Verified,
                Issuer = "No Image Issuer"
            };
            var handler = new StoreVerifiedCredentialHandler(loggerMock.Object, Fixture.ServiceScopeFactory);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess();
            result.Value.Should().NotBeNull();
            result.Value.TemplateId.Should().Be("noimage_no_description_light",
                "because the handler sets the template to noimage_no_description_light when no image is provided");
        }

        [Fact]
        public async Task Handle_WithDomainAndClaims_ShouldPersistDataInDatabase()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StoreVerifiedCredentialHandler>>();
            var request = new StoreVerifiedCredentialRequest
            {
                Image = "https://example.com/image2.jpg",
                Credential = "{ \"some\": \"credential data 2\" }",
                Status = EVerificationStatus.Verified,
                Issuer = "Issuer With Domain",
                Domain = "example.org",
                Claims = new Dictionary<string, string>
                {
                    { "role", "admin" },
                    { "level", "high" }
                }
            };
            var handler = new StoreVerifiedCredentialHandler(loggerMock.Object, Fixture.ServiceScopeFactory);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeSuccess();
            result.Value.Should().NotBeNull();
            result.Value.Domain.Should().Be("example.org");
            result.Value.Claims.Should().NotBeNull();
            result.Value.Claims!.Count.Should().Be(2);
            result.Value.Claims["role"].Should().Be("admin");
            result.Value.Claims["level"].Should().Be("high");

            using var scope = Fixture.ServiceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var credentialInDb = await context.Set<VerifiedCredentialEntity>()
                .FirstOrDefaultAsync(c => c.Domain == "example.org");

            credentialInDb.Should().NotBeNull();
            credentialInDb!.Domain.Should().Be("example.org");
            credentialInDb.Claims.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenExceptionIsThrown_ShouldReturnFailureResult()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StoreVerifiedCredentialHandler>>();

            // Create a scope factory mock that throws an exception on SaveChangesAsync
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var contextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            serviceScopeFactoryMock
                .Setup(f => f.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceScopeMock
                .Setup(s => s.ServiceProvider.GetService(typeof(ApplicationDbContext)))
                .Returns(contextMock.Object);

            contextMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Simulated DB exception"));

            var request = new StoreVerifiedCredentialRequest
            {
                Credential = "{ \"data\": \"will fail\" }",
                Status = EVerificationStatus.Verified,
                Issuer = "Failing Issuer"
            };

            var handler = new StoreVerifiedCredentialHandler(loggerMock.Object, serviceScopeFactoryMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeFailure();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Be("An error occurred while storing the credential");
        }
    }
}
