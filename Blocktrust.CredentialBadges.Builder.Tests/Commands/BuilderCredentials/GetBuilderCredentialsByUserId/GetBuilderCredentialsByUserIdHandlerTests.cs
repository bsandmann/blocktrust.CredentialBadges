using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.BuilderCredentials.GetBuilderCredentialsByUserId;

using Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Test for fetching builder credentials by user id and optionally by subject did
/// </summary>
public class GetBuilderCredentialsByUserIdHandlerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialsByUserIdHandler> _logger;
    private readonly GetBuilderCredentialsByUserIdHandler _handler;

    public GetBuilderCredentialsByUserIdHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql("Host=10.10.20.103; Database=CredentialBadgesTests; Username=postgres; Password=postgres"));

        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        _context.Database.EnsureCreated();

        _logger = _serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<GetBuilderCredentialsByUserIdHandler>();
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _handler = new GetBuilderCredentialsByUserIdHandler(_logger, scopeFactory);
    }

    // Clean up after each test
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _serviceProvider.Dispose();
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
}