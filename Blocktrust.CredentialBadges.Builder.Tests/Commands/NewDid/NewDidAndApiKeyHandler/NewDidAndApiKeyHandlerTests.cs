// using Blocktrust.CredentialBadges.Builder.Commands.NewDid;
// using Blocktrust.CredentialBadges.Builder.Common;
// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid.NewDidAndApiKey;
//
// /// <summary>
// /// Tests for creating a new DID and API key
// /// </summary>
// public class NewDidAndApiKeyHandlerTests
// {
//     private readonly IOptions<AppSettings> _appSettings;
//     private readonly ILogger<NewDidAndApiKeyHandler> _logger;
//     private readonly TestHttpClientFactory _httpClientFactory;
//
//     /// <summary>
//     /// Constructor to setup the test environment
//     /// </summary>
//     public NewDidAndApiKeyHandlerTests()
//     {
//         // Setup service collection
//         var services = new ServiceCollection();
//         services.AddLogging();
//
//         // Configure AppSettings directly
//         services.Configure<AppSettings>(options =>
//         {
//             options.UserAgentBaseUrl = "http://212.124.52.36:35412/cloud-agent/";
//             options.UserAgentAdminKey = "adminsec";
//         });
//
//         // Build service provider
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Get services
//         _appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
//         _logger = serviceProvider.GetRequiredService<ILogger<NewDidAndApiKeyHandler>>();
//
//         // Initialize HttpClientFactory
//         _httpClientFactory = new TestHttpClientFactory(_appSettings);
//     }
//
//     /// <summary>
//     /// Test to ensure creating a new DID and API key is successful
//     /// </summary>
//     [Fact]
//     public async Task Handle_ShouldReturnDidAndApiKey_WhenValidRequestIsProvided()
//     {
//         // Arrange
//         var handler = new NewDidAndApiKeyHandler(_httpClientFactory, _appSettings); // Adjusted constructor parameters
//         var request = new NewDidAndApiKeyRequest
//         {
//             Seed = TestHelpers.GenerateRandomHexString(128), // Generate a random seed
//             WalletName = TestHelpers.GenerateRandomName(10)   // Generate a random name
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsSuccess.Should().BeTrue();
//         result.Value.Should().NotBeNull();
//         result.Value.Did.Should().NotBeNullOrEmpty();
//         result.Value.ApiKey.Should().NotBeNullOrEmpty();
//     }
//
//     /// <summary>
//     /// Test to ensure creating a new DID and API key fails when seed is empty
//     /// </summary>
//     [Fact]
//     public async Task Handle_ShouldReturnFailResult_WhenSeedIsEmpty()
//     {
//         // Arrange
//         var handler = new NewDidAndApiKeyHandler(_httpClientFactory, _appSettings); // Adjusted constructor parameters
//         var request = new NewDidAndApiKeyRequest
//         {
//             Seed = string.Empty, // Invalid seed
//             WalletName = TestHelpers.GenerateRandomName(10)
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsFailed.Should().BeTrue();
//         result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters"));
//     }
//
//     /// <summary>
//     /// Test to ensure creating a new DID and API key fails when wallet name is empty
//     /// </summary>
//     [Fact]
//     public async Task Handle_ShouldReturnFailResult_WhenWalletNameIsEmpty()
//     {
//         // Arrange
//         var handler = new NewDidAndApiKeyHandler(_httpClientFactory, _appSettings); // Adjusted constructor parameters
//         var request = new NewDidAndApiKeyRequest
//         {
//             Seed = TestHelpers.GenerateRandomHexString(128),
//             WalletName = string.Empty // Invalid wallet name
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsFailed.Should().BeTrue();
//         result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters"));
//     }
//
//     /// <summary>
//     /// Test to ensure creating a new DID and API key fails when both parameters are empty
//     /// </summary>
//     [Fact]
//     public async Task Handle_ShouldReturnFailResult_WhenBothParametersAreEmpty()
//     {
//         // Arrange
//         var handler = new NewDidAndApiKeyHandler(_httpClientFactory, _appSettings); // Adjusted constructor parameters
//         var request = new NewDidAndApiKeyRequest
//         {
//             Seed = string.Empty, // Invalid seed
//             WalletName = string.Empty // Invalid wallet name
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsFailed.Should().BeTrue();
//         result.Errors.Should().ContainSingle(e => e.Message.Contains("Invalid request parameters"));
//     }
// }
