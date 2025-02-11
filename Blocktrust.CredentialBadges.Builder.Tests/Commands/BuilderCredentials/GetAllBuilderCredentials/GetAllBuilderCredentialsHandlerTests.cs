using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Tests.Commands.BuilderCredentials;

public class GetAllBuilderCredentialsHandlerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetAllBuilderCredentialsHandler> _logger;
    private readonly GetAllBuilderCredentialsHandler _handler;

    public GetAllBuilderCredentialsHandlerTests()
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
            .CreateLogger<GetAllBuilderCredentialsHandler>();
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _handler = new GetAllBuilderCredentialsHandler(_logger, scopeFactory);
    }

    /// <summary>
    ///  Clean up after each test
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _serviceProvider.Dispose();
    }

    /// <summary>
    ///  Test to get all builder credentials when credentials exist
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnAllBuilderCredentials_WhenCredentialsExist()
    {
        // Arrange
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
                UserId = "testuser1",
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
                UserId = "testuser2",
                ThId = Guid.NewGuid(),
                RecordIdOnAgent = Guid.NewGuid(),
                VerifiableCredential = "Test VC 2"
            }
        };

        // Seed data into the test DbContext
        await _context.BuilderCredentials.AddRangeAsync(credentials);
        await _context.SaveChangesAsync();

        var request = new GetAllBuilderCredentialsRequest();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCountGreaterOrEqualTo(2);
    }

    /// <summary>
    ///  Test to get all builder credentials when no credentials exist
    /// </summary>
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

}
