using Blocktrust.CredentialBadges.Builder.Commands.NewDid;
using Blocktrust.CredentialBadges.Builder.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid.RegisterApiKeyHandler;

/// <summary>
/// Tests for API key registration
/// </summary>
public class RegisterApiKeyHandlerTests
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<Builder.Commands.NewDid.RegisterApiKeyHandler> _apiKeyLogger;
    private readonly ILogger<RegisterWalletHandler> _walletLogger;
    private readonly ILogger<RegisterEntityHandler> _entityLogger;
    private readonly TestHttpClientFactory _httpClientFactory;

    public RegisterApiKeyHandlerTests()
    {
        // Setup service collection
        var services = new ServiceCollection();
        services.AddLogging();
            
        // Configure AppSettings directly
        services.Configure<AppSettings>(options =>
        {
            options.UserAgentBaseUrl = "http://212.124.52.36:35412/cloud-agent/";
            options.UserAgentAdminKey = "adminsec";
        });

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get services
        _appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        _apiKeyLogger = serviceProvider.GetRequiredService<ILogger<Builder.Commands.NewDid.RegisterApiKeyHandler>>();
        _walletLogger = serviceProvider.GetRequiredService<ILogger<RegisterWalletHandler>>();
        _entityLogger = serviceProvider.GetRequiredService<ILogger<RegisterEntityHandler>>();

        // Initialize HttpClientFactory
        _httpClientFactory = new TestHttpClientFactory(_appSettings);
    }

    /// <summary>
    /// Test to ensure API key registration is successful
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnSuccessMessage_WhenApiKeyIsRegisteredSuccessfully()
    {
        // Arrange
        var walletHandler = new RegisterWalletHandler(_httpClientFactory, _appSettings, _walletLogger);
        var entityHandler = new RegisterEntityHandler(_httpClientFactory, _appSettings, _entityLogger);
        var apiKeyHandler = new Builder.Commands.NewDid.RegisterApiKeyHandler(_httpClientFactory, _appSettings, _apiKeyLogger);

        // Step 1: Register a wallet and get its ID
        var walletRequest = new RegisterWalletRequest
        {
            Seed = TestHelpers.GenerateRandomHexString(128),
            Name = TestHelpers.GenerateRandomName(10)
        };
        var walletResult = await walletHandler.Handle(walletRequest, CancellationToken.None);
        walletResult.IsSuccess.Should().BeTrue();
        var walletId = walletResult.Value;

        // Step 2: Register an entity and get its ID
        var entityRequest = new RegisterEntityRequest
        {
            Name = TestHelpers.GenerateRandomName(10),
            WalletId = walletId
        };
        var entityResult = await entityHandler.Handle(entityRequest, CancellationToken.None);
        entityResult.IsSuccess.Should().BeTrue();
        var entityId = entityResult.Value;

        // Step 3: Register an API key
        var request = new RegisterApiKeyRequest
        {
            EntityId = entityId,
            ApiKey = TestHelpers.GenerateRandomApiKey(32)
        };

        // Act
        var result = await apiKeyHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("API key registered successfully");
    }

    /// <summary>
    /// Test to ensure API key registration fails when invalid parameters are used
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenInvalidParametersAreUsed()
    {
        // Arrange
        var handler = new Builder.Commands.NewDid.RegisterApiKeyHandler(_httpClientFactory, _appSettings, _apiKeyLogger);

        var request = new RegisterApiKeyRequest
        {
            EntityId = Guid.Empty, // Invalid EntityId
            ApiKey = string.Empty  // Invalid ApiKey
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters\n\nStatus: 400\nResponse: \n"));
    }
}