using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.CreateBuilderCredential;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blocktrust.CredentialBadges.Tests.Commands.BuilderCredentials;

/// <summary>
///  Test for creating a builder credential
/// </summary>
public class CreateBuilderCredentialHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateBuilderCredentialHandler> _logger;
    private readonly CreateBuilderCredentialHandler _handler;
    /// <summary>
    ///     Constructor to initialize the test class
    /// </summary>
    public CreateBuilderCredentialHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost; Database=BuilderDatabase; Username=postgres; Password=Post@0DB")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
        _logger = new LoggerFactory().CreateLogger<CreateBuilderCredentialHandler>();
        _handler = new CreateBuilderCredentialHandler(_context, _logger);
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
    ///   Test to create a builder credential
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBuilderCredential_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateBuilderCredentialRequest
        {
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

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CredentialId.Should().NotBeEmpty();
        result.Value.Date.Should().Be(request.Date);
        result.Value.Label.Should().Be(request.Label);
        result.Value.SubjectDid.Should().Be(request.SubjectDid);
        result.Value.IssuerDid.Should().Be(request.IssuerDid);
        result.Value.Status.Should().Be(request.Status);
        result.Value.IssuerConnectionId.Should().Be(request.IssuerConnectionId);
        result.Value.SubjectConnectionId.Should().Be(request.SubjectConnectionId);
        result.Value.CredentialSubject.Should().Be(request.CredentialSubject);
        result.Value.UserId.Should().Be(request.UserId);
        result.Value.ThId.Should().Be(request.ThId);
        result.Value.RecordIdOnAgent.Should().Be(request.RecordIdOnAgent);
        result.Value.VerifiableCredential.Should().Be(request.VerifiableCredential);

        // Verify that the credential was added to the database
        var savedCredential = await _context.BuilderCredentials.FirstOrDefaultAsync(c => c.CredentialId == result.Value.CredentialId);
        savedCredential.Should().NotBeNull();
    }
    /// <summary>
    ///  Test to create a builder credential with invalid data
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenInvalidDataProvided()
    {
        // Arrange
        var request = new CreateBuilderCredentialRequest
        {
            // Provide invalid or missing data
            Date = DateTime.MinValue, // Invalid date
            Label = "", // Empty label
            SubjectDid = null, // Null SubjectDid
            IssuerDid = "", // Empty IssuerDid
            Status = (EBuilderCredentialStatus)999, // Invalid status
            IssuerConnectionId = Guid.Empty, // Empty GUID
            SubjectConnectionId = Guid.Empty, // Empty GUID
            CredentialSubject = null, // Null CredentialSubject
            UserId = "", // Empty UserId
            ThId = Guid.Empty, // Empty GUID
            RecordIdOnAgent = Guid.Empty, // Empty GUID
            VerifiableCredential = null // Null VerifiableCredential
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }
}