using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.BuilderCredentials.GetBuilderCredentialsByUserId;

using Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;

/// <summary>
/// Test for fetching builder credentials by user id and optionally by subject did
/// </summary>
public class GetBuilderCredentialsByUserIdHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialsByUserIdHandler> _logger;
    private readonly GetBuilderCredentialsByUserIdHandler _handler;
    
    /// <summary>
    /// Constructor to initialize the test class
    /// </summary>
    public GetBuilderCredentialsByUserIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost; Database=BuilderDatabase; Username=postgres; Password=Post@0DB")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _logger = new LoggerFactory().CreateLogger<GetBuilderCredentialsByUserIdHandler>();
        _handler = new GetBuilderCredentialsByUserIdHandler(_context, _logger);
    }

    /// <summary>
    ///  Clean up the test class
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    ///  Test to get all builder credentials by user id
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnAllBuilderCredentials_WhenOnlyUserIdProvided()
    {
        // Arrange
        var userId = "testuser";
        var credentials = new List<BuilderCredentialEntity>
        {
            new()
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
                UserId = userId,
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 1"
            },
            new()
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
                UserId = userId,
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 2"
            }
        };

        await _context.BuilderCredentials.AddRangeAsync(credentials);
        await _context.SaveChangesAsync();

        var request = new GetBuilderCredentialsByUserIdRequest(userId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].UserId.Should().Be(userId);
        result.Value[1].UserId.Should().Be(userId);
    }

    /// <summary>
    ///  Test to get builder credentials by user id and subject did
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFilteredBuilderCredentials_WhenUserIdAndSubjectDidProvided()
    {
        // Arrange
        var userId = "testuser";
        var subjectDid = "did:example:subject1";
        var credentials = new List<BuilderCredentialEntity>
        {
            new()
            {
                CredentialId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Label = "Test Label 1",
                SubjectDid = subjectDid,
                IssuerDid = "did:example:issuer1",
                Status = EBuilderCredentialStatus.Pending,
                IssuerConnectionId = Guid.NewGuid(),
                SubjectConnectionId = Guid.NewGuid(),
                CredentialSubject = "Test Subject 1",
                UserId = userId,
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 1"
            },
            new()
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
                UserId = userId,
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 2"
            }
        };

        await _context.BuilderCredentials.AddRangeAsync(credentials);
        await _context.SaveChangesAsync();

        var request = new GetBuilderCredentialsByUserIdRequest(userId, subjectDid);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].UserId.Should().Be(userId);
        result.Value[0].SubjectDid.Should().Be(subjectDid);
    }

    /// <summary>
    ///  Test to get all builder credentials by user id
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCredentialsExistForUserId()
    {
        // Arrange
        var request = new GetBuilderCredentialsByUserIdRequest("nonexistentuser");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    /// <summary>
    ///  Test to get all builder credentials by user id and subject did
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCredentialsExistForUserIdAndSubjectDid()
    {
        // Arrange
        var request = new GetBuilderCredentialsByUserIdRequest("nonexistentuser", "nonexistentdid");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    /// <summary>
    ///  Test to get all builder credentials by user id
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        // Arrange
        var faultyHandler = new GetBuilderCredentialsByUserIdHandler(null, _logger);
        var request = new GetBuilderCredentialsByUserIdRequest("testuser");

        // Act
        var result = await faultyHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }
}