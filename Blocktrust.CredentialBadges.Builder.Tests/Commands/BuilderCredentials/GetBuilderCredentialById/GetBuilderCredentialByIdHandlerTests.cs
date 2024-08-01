using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialById;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.BuilderCredentials.GetBuilderCredentialById;
/// <summary>
/// Test for fetching builder credential by id
/// </summary>
public class GetBuilderCredentialByIdHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialByIdHandler> _logger;
    private readonly GetBuilderCredentialByIdHandler _handler;

    /// <summary>
    ///  Constructor to initialize the test class
    /// </summary>
    public GetBuilderCredentialByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost; Database=BuilderDatabase; Username=postgres; Password=Post@0DB")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _logger = new LoggerFactory().CreateLogger<GetBuilderCredentialByIdHandler>();
        _handler = new GetBuilderCredentialByIdHandler(_context, _logger);
    }

    /// <summary>
    ///     Clean up the test class
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    ///   Test to get builder credential by id when credential exists
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnBuilderCredential_WhenCredentialExists()
    {
        // Arrange
        var credentialId = Guid.NewGuid();
        var credential = new BuilderCredentialEntity()
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

        await _context.BuilderCredentials.AddAsync(credential);
        await _context.SaveChangesAsync();

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
    ///  Test to get builder credential by id with invalid context
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        // Arrange
        var faultyHandler = new GetBuilderCredentialByIdHandler(null, _logger);
        var request = new GetBuilderCredentialByIdRequest(Guid.NewGuid());

        // Act
        var result = await faultyHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Failed to retrieve builder credential");
    }
}