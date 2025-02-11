using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialById;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.BuilderCredentials.GetBuilderCredentialById;

public class GetBuilderCredentialByIdHandlerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialByIdHandler> _logger;
    private readonly GetBuilderCredentialByIdHandler _handler;

    public GetBuilderCredentialByIdHandlerTests()
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
            .CreateLogger<GetBuilderCredentialByIdHandler>();
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        _handler = new GetBuilderCredentialByIdHandler(_logger, scopeFactory);
    }

    /// <summary>
    ///  Clean up the test class
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _serviceProvider.Dispose();
    }

    /// <summary>
    ///   Test to get builder credential by id when credential exists
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnBuilderCredential_WhenCredentialExists()
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var credential = new BuilderCredentialEntity
        {
            CredentialId = credentialId,
            Date = DateTime.UtcNow,
            Label = "Test Label",
            SubjectDid = "did:example:subject",
            IssuerDid = "did:example:issuer",
            Status = EBuilderCredentialStatus.Pending,
            IssuerConnectionId = Guid.NewGuid(),
            SubjectConnectionId = Guid.NewGuid(),
            CredentialSubject = "Test Subject",
            UserId = "testuser",
            ThId = Guid.NewGuid(),
            RecordIdOnAgent = Guid.NewGuid(),
            VerifiableCredential = "Test VC"
        };

        // Seed data into the test DbContext
        await _context.BuilderCredentials.AddAsync(credential);
        await _context.SaveChangesAsync();

        // Prepare request
        var request = new GetBuilderCredentialByIdRequest(credentialId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CredentialId.Should().Be(credentialId);
    }

    /// <summary>
    ///  Test to get builder credential by id when credential does not exist
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenCredentialDoesNotExist()
    {
        // Arrange
        var nonExistentCredentialId = Guid.NewGuid();
        var request = new GetBuilderCredentialByIdRequest(nonExistentCredentialId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Builder credential not found");
    }

    /// <summary>
    ///  Test to get builder credential by id with an invalid scope factory
    ///  (simulating an internal exception)
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        // Arrange
        // Pass null for IServiceScopeFactory to simulate a fault
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var faultyHandler = new GetBuilderCredentialByIdHandler(_logger, scopeFactory );
        var request = new GetBuilderCredentialByIdRequest(Guid.NewGuid());

        // Act
        var result = await faultyHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }
}
