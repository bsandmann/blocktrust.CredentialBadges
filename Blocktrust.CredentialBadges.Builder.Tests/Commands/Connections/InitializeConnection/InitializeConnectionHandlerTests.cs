using Blocktrust.CredentialBadges.Builder.Commands.Connections;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.Builder.Tests.Commands.Connections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Tests.Commands.Connections;

using Builder.Commands.Connections.InitializeConnection;

/// <summary>
///  Tests for initializing connection
/// </summary>
public class InitializeConnectionHandlerTests
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<InitializeConnectionHandler> _logger;
    private readonly TestHttpClientFactory _httpClientFactory;
    /// <summary>
    ///     Constructor to initialize the test class
    /// </summary>
    public InitializeConnectionHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
            
        services.Configure<AppSettings>(options =>
        {
            options.AdminAgentBaseUrl = "http://212.124.52.36:35412/cloud-agent/";
            options.AdminApiKey = "597634cfaa4feae5";
        });

        var serviceProvider = services.BuildServiceProvider();

        _appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        _logger = serviceProvider.GetRequiredService<ILogger<InitializeConnectionHandler>>();
        _httpClientFactory = new TestHttpClientFactory(_appSettings);
    }

    // [Fact]
    // public async Task Handle_ShouldReturnSuccessResult_WhenConnectionCreatedSuccessfully()
    // {
    //     // Arrange
    //     var handler = new InitializeConnectionHandler(_httpClientFactory, _appSettings, _logger);
    //     var request = new InitializeConnectionRequest();
    //
    //     // Act
    //     var result = await handler.Handle(request, CancellationToken.None);
    //
    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    //     result.Value.Should().NotBeNull();
    //     result.Value.ConnectionId.Should().NotBeNullOrEmpty();
    //     result.Value.InvitationUrl.Should().NotBeNullOrEmpty();
    // }

    /// <summary>
    ///  Test to get builder credential by id when credential exists
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        // Arrange
        _httpClientFactory.ShouldThrowException = true;
        var handler = new InitializeConnectionHandler(_httpClientFactory, _appSettings, _logger);
        var request = new InitializeConnectionRequest();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Test exception");
    }

    /// <summary>
    ///  Test to get builder credential by id with invalid context
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUseCorrectApiKey()
    {
        // Arrange
        var handler = new InitializeConnectionHandler(_httpClientFactory, _appSettings, _logger);
        var request = new InitializeConnectionRequest();

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        _httpClientFactory.LastCreatedClient.DefaultRequestHeaders.GetValues("apiKey").Should().ContainSingle().Which.Should().Be("597634cfaa4feae5");
    }
}


