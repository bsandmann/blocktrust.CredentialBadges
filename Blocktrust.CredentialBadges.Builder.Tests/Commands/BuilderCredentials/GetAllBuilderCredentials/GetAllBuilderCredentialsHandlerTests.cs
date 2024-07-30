using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Blocktrust.CredentialBadges.Builder.Enums;

namespace Blocktrust.CredentialBadges.Tests.Commands.BuilderCredentials;

public class GetAllBuilderCredentialsHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetAllBuilderCredentialsHandler> _logger;
    private readonly GetAllBuilderCredentialsHandler _handler;

    public GetAllBuilderCredentialsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost; Database=BuilderDatabase; Username=postgres; Password=Post@0DB")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _logger = new LoggerFactory().CreateLogger<GetAllBuilderCredentialsHandler>();
        _handler = new GetAllBuilderCredentialsHandler(_context, _logger);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllBuilderCredentials_WhenCredentialsExist()
    {
        // Arrange
        var credentials = new List<BuilderCredentialEntity>
        {
            new BuilderCredentialEntity
            {
                CredentialId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Label = "Test Label 1",
                SubjectDid = "did:example:subject1",
                IssuerDid = "did:example:issuer1",
                Status = EBuilderCredentialStatus.Pending,
                IssuerConnectionId = Guid.NewGuid(),
                SubjectConnectionId = Guid.NewGuid(),
                CredentialSubject = "Test Subject 1",
                UserId = "testuser1",
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 1"
            },
            new BuilderCredentialEntity
            {
                CredentialId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Label = "Test Label 2",
                SubjectDid = "did:example:subject2",
                IssuerDid = "did:example:issuer2",
                Status = EBuilderCredentialStatus.Pending,
                IssuerConnectionId = Guid.NewGuid(),
                SubjectConnectionId = Guid.NewGuid(),
                CredentialSubject = "Test Subject 2",
                UserId = "testuser2",
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 2"
            }
        };

        await _context.BuilderCredentials.AddRangeAsync(credentials);
        await _context.SaveChangesAsync();

        var request = new GetAllBuilderCredentialsRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        //COUNT TWO OR MORE 
        result.Value.Should().HaveCountGreaterOrEqualTo(2);
     
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCredentialsExist()
    {
        // Arrange
        var request = new GetAllBuilderCredentialsRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        // Arrange
        var handlerWithFaultyContext = new GetAllBuilderCredentialsHandler(null, _logger);
        var request = new GetAllBuilderCredentialsRequest();

        // Act
        var result = await handlerWithFaultyContext.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }
}