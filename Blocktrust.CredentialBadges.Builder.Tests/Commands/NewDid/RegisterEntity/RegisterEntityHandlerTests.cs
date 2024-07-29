using Blocktrust.CredentialBadges.Builder.Commands.NewDid;
using Blocktrust.CredentialBadges.Builder.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid.RegisterEntity;

/// <summary>
/// Integration tests for entity registration
/// </summary>

public class RegisterEntityHandlerTests
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<RegisterEntityHandler> _logger;
    private readonly ILogger<RegisterWalletHandler> _walletLogger;
    private readonly TestHttpClientFactory _httpClientFactory;
    /// <summary>
    ///  Constructor to setup the test environment
    /// </summary>
        public RegisterEntityHandlerTests()
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
        _logger = serviceProvider.GetRequiredService<ILogger<RegisterEntityHandler>>();
        _walletLogger = serviceProvider.GetRequiredService<ILogger<RegisterWalletHandler>>();

        // Initialize HttpClientFactory
        _httpClientFactory = new TestHttpClientFactory(_appSettings);
    }
    /// <summary>
    ///  Method to generate a random name
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private string GenerateRandomName(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    /// <summary>
    /// Method to register a wallet under which an entity will be registered
    /// </summary>
    /// <returns>wallet id</returns>
    private async Task<Guid> RegisterWalletAsync()
    {
        var registerWalletHandler = new RegisterWalletHandler(_httpClientFactory, _appSettings, _walletLogger);
        var request = new RegisterWalletRequest
        {
            Seed = GenerateRandomHexString(128), // Generate a random seed
            Name = GenerateRandomName(10)         // Generate a random name
        };

        var result = await registerWalletHandler.Handle(request, CancellationToken.None);
        result.IsSuccess.Should().BeTrue(); // Ensure wallet registration was successful
        return result.Value;
    }
    /// <summary>
    ///  Method to generate a random hex string
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private string GenerateRandomHexString(int length)
    {
        var random = new Random();
        var buffer = new byte[length / 2];
        random.NextBytes(buffer);
        return BitConverter.ToString(buffer).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Test for successful entity registration
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnEntityId_WhenEntityIsCreatedSuccessfully()
    {
        // Arrange
        var walletId = await RegisterWalletAsync(); // Register wallet first
        var handler = new RegisterEntityHandler(_httpClientFactory, _appSettings, _logger);
        var request = new RegisterEntityRequest
        {
            Name = GenerateRandomName(10),
            WalletId = walletId // Use the registered wallet ID
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }
    /// <summary>
    /// Test for entity registration failure
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenEntityCreationFails()
    {
        // Arrange
        //new guid 
        var walletId = Guid.NewGuid();
        var handler = new RegisterEntityHandler(_httpClientFactory, _appSettings, _logger);
        var request = new RegisterEntityRequest
        {
            Name = GenerateRandomName(10),
            WalletId = walletId // Use the registered wallet ID
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters\n\nStatus: 400\nResponse: \n"));
    }
}
