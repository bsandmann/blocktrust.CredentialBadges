using Blocktrust.CredentialBadges.Builder.Commands.NewDid;
using Blocktrust.CredentialBadges.Builder.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid.RegisterWallet;

/// <summary>
/// Tests for wallet registration
/// </summary>
public class RegisterWalletHandlerTests
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<RegisterWalletHandler> _logger;
    private readonly TestHttpClientFactory _httpClientFactory;
    
    /// <summary>
    ///  Constructor to setup the test environment
    /// </summary>
    public RegisterWalletHandlerTests()
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
        _logger = serviceProvider.GetRequiredService<ILogger<RegisterWalletHandler>>();

        // Initialize HttpClientFactory
        _httpClientFactory = new TestHttpClientFactory(_appSettings);
    }
    

    /// <summary>
    /// Test to ensure wallet registration is successful
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnWalletId_WhenWalletIsCreatedSuccessfully()
    {
        // Arrange
        var handler = new RegisterWalletHandler(_httpClientFactory, _appSettings, _logger);
        var request = new RegisterWalletRequest
        {
            Seed = TestHelpers.GenerateRandomHexString(128),  // Generate a random seed
            Name = TestHelpers.GenerateRandomName(10)       // Generate a random name
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    /// <summary>
    /// Test to ensure wallet registration fails when invalid parameters are used
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenInvalidParametersAreUsed()
    {
        // Arrange
        var handler = new RegisterWalletHandler(_httpClientFactory, _appSettings, _logger);
            
        // Use an invalid seed (incorrect length) 
        var request = new RegisterWalletRequest
        {
            Seed = TestHelpers.GenerateRandomHexString(10),  // Generate a random seed with invalid length
            Name = TestHelpers.GenerateRandomName(5)      
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters\n\nStatus: 400\nResponse: \n"));
    }
}