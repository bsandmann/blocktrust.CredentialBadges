using Blocktrust.CredentialBadges.Builder.Commands.NewDid;
using Blocktrust.CredentialBadges.Builder.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class RegisterWalletHandlerTests
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<RegisterWalletHandler> _logger;

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

        // Configure HttpClient
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_appSettings.Value.UserAgentBaseUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.Value.UserAgentAdminKey);
    }

    private string GenerateRandomHexString(int length)
    {
        var random = new Random();
        var buffer = new byte[length / 2];
        random.NextBytes(buffer);
        return BitConverter.ToString(buffer).Replace("-", "").ToLower();
    }

    private string GenerateRandomName(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldReturnWalletId_WhenWalletIsCreatedSuccessfully()
    {
        // Arrange
        var handler = new RegisterWalletHandler(new HttpClientFactory(_httpClient), _appSettings, _logger);
        var request = new RegisterWalletRequest
        {
            Seed = GenerateRandomHexString(128),  // Generate a random seed
            Name = GenerateRandomName(10)       // Generate a random name
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenInvalidParametersAreUsed()
    {
        // Arrange
        var handler = new RegisterWalletHandler(new HttpClientFactory(_httpClient), _appSettings, _logger);
        
        // Use an invalid seed (incorrect length) 
        var request = new RegisterWalletRequest
        {
            Seed = GenerateRandomHexString(10),  // Generate a random seed with invalid length
            Name = GenerateRandomName(5)      
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters\n\nStatus: 400\nResponse: \n"));
    }
}

public class HttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _httpClient;

    public HttpClientFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient CreateClient(string name)
    {
        return _httpClient;
    }
}
