// using Blocktrust.CredentialBadges.Builder.Commands.Connections;
// using Blocktrust.CredentialBadges.Builder.Common;
// using Blocktrust.CredentialBadges.Builder.Tests.Commands.Connections;
// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace Blocktrust.CredentialBadges.Tests.Commands.Connections;
//
// public class AcceptConnectionHandlerTests
// {
//     private readonly IOptions<AppSettings> _appSettings;
//     private readonly ILogger<AcceptConnectionHandler> _logger;
//     private readonly TestHttpClientFactory _httpClientFactory;
//
//     public AcceptConnectionHandlerTests()
//     {
//         var services = new ServiceCollection();
//         services.AddLogging();
//
//         services.Configure<AppSettings>(options =>
//         {
//             options.UserAgentBaseUrl = "http://212.124.52.36:35413/cloud-agent/";
//         });
//
//         var serviceProvider = services.BuildServiceProvider();
//
//         _appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
//         _logger = serviceProvider.GetRequiredService<ILogger<AcceptConnectionHandler>>();
//         _httpClientFactory = new TestHttpClientFactory(_appSettings);
//     }
//
//     [Fact]
//     public async Task Handle_ShouldReturnSuccessResult_WhenConnectionAcceptedSuccessfully()
//     {
//         // Arrange
//         var handler = new AcceptConnectionHandler(_httpClientFactory, _appSettings, _logger);
//         var request = new AcceptConnectionRequest
//         {
//             InvitationUrl = "eyJpZCI6Ijk0YjFkYzA1LWUzYjctNGQ4MS05YWNiLTI2Nzg0NjAzMmVkMyIsInR5cGUiOiJodHRwczovL2RpZGNvbW0ub3JnL291dC1vZi1iYW5kLzIuMC9pbnZpdGF0aW9uIiwiZnJvbSI6ImRpZDpwZWVyOjIuRXo2TFNtQjdCMTVpRkIxRXpHS3diN1dzUXBNdDJzSlJ5ZjI5Q0JrTG5uQzhzRVZvcC5WejZNa3BVWEJISlhob2tyU3F2OVpyU1JITWlEVnkzbnp2d0J2NkJBYUYxV0s5WXhSLlNleUowSWpvaVpHMGlMQ0p6SWpwN0luVnlhU0k2SW1oMGRIQTZMeTh4TUM0eE1DNDFNQzR4TURVNk9EQXdNQzlrYVdSamIyMXRJaXdpY2lJNlcxMHNJbUVpT2xzaVpHbGtZMjl0YlM5Mk1pSmRmWDAiLCJib2R5Ijp7ImFjY2VwdCI6W119fQ",
//             ApiKey = "XVSjbfrurEDnj4QE"
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsSuccess.Should().BeTrue();
//         result.Value.Should().NotBeNull();
//     }
//
//     [Fact]
//     public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
//     {
//         // Arrange
//         _httpClientFactory.ShouldThrowException = true;
//         var handler = new AcceptConnectionHandler(_httpClientFactory, _appSettings, _logger);
//         var request = new AcceptConnectionRequest
//         {
//             InvitationUrl = "eyJpZCI6Ijk0YjFkYzA1LWUzYjctNGQ4MS05YWNiLTI2Nzg0NjAzMmVkMyIsInR5cGUiOiJodHRwczovL2RpZGNvbW0ub3JnL291dC1vZi1iYW5kLzIuMC9pbnZpdGF0aW9uIiwiZnJvbSI6ImRpZDpwZWVyOjIuRXo2TFNtQjdCMTVpRkIxRXpHS3diN1dzUXBNdDJzSlJ5ZjI5Q0JrTG5uQzhzRVZvcC5WejZNa3BVWEJISlhob2tyU3F2OVpyU1JITWlEVnkzbnp2d0J2NkJBYUYxV0s5WXhSLlNleUowSWpvaVpHMGlMQ0p6SWpwN0luVnlhU0k2SW1oMGRIQTZMeTh4TUM0eE1DNDFNQzR4TURVNk9EQXdNQzlrYVdSamIyMXRJaXdpY2lJNlcxMHNJbUVpT2xzaVpHbGtZMjl0YlM5Mk1pSmRmWDAiLCJib2R5Ijp7ImFjY2VwdCI6W119fQ==",
//             ApiKey = "XVSjbfrurEDnj4QE"
//         };
//
//         // Act
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         result.IsFailed.Should().BeTrue();
//         result.Errors.Should().ContainSingle();
//     }
//
//     [Fact]
//     public async Task Handle_ShouldUseProvidedApiKey_WhenApiKeyIsProvided()
//     {
//         // Arrange
//         var handler = new AcceptConnectionHandler(_httpClientFactory, _appSettings, _logger);
//         var request = new AcceptConnectionRequest
//         {
//             InvitationUrl = "eyJpZCI6Ijk0YjFkYzA1LWUzYjctNGQ4MS05YWNiLTI2Nzg0NjAzMmVkMyIsInR5cGUiOiJodHRwczovL2RpZGNvbW0ub3JnL291dC1vZi1iYW5kLzIuMC9pbnZpdGF0aW9uIiwiZnJvbSI6ImRpZDpwZWVyOjIuRXo2TFNtQjdCMTVpRkIxRXpHS3diN1dzUXBNdDJzSlJ5ZjI5Q0JrTG5uQzhzRVZvcC5WejZNa3BVWEJISlhob2tyU3F2OVpyU1JITWlEVnkzbnp2d0J2NkJBYUYxV0s5WXhSLlNleUowSWpvaVpHMGlMQ0p6SWpwN0luVnlhU0k2SW1oMGRIQTZMeTh4TUM0eE1DNDFNQzR4TURVNk9EQXdNQzlrYVdSamIyMXRJaXdpY2lJNlcxMHNJbUVpT2xzaVpHbGtZMjl0YlM5Mk1pSmRmWDAiLCJib2R5Ijp7ImFjY2VwdCI6W119fQ==",
//             ApiKey = "XVSjbfrurEDnj4QE"
//
//         };
//
//         // Act
//         await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         _httpClientFactory.LastCreatedClient.DefaultRequestHeaders.GetValues("apiKey").Should().ContainSingle()
//             .Which.Should().Be("custom-api-key");
//     }
//
//     [Fact]
//     public async Task Handle_ShouldNotAddApiKeyHeader_WhenApiKeyIsNotProvided()
//     {
//         // Arrange
//         var handler = new AcceptConnectionHandler(_httpClientFactory, _appSettings, _logger);
//         var request = new AcceptConnectionRequest
//         {
//             InvitationUrl = "eyJpZCI6Ijk0YjFkYzA1LWUzYjctNGQ4MS05YWNiLTI2Nzg0NjAzMmVkMyIsInR5cGUiOiJodHRwczovL2RpZGNvbW0ub3JnL291dC1vZi1iYW5kLzIuMC9pbnZpdGF0aW9uIiwiZnJvbSI6ImRpZDpwZWVyOjIuRXo2TFNtQjdCMTVpRkIxRXpHS3diN1dzUXBNdDJzSlJ5ZjI5Q0JrTG5uQzhzRVZvcC5WejZNa3BVWEJISlhob2tyU3F2OVpyU1JITWlEVnkzbnp2d0J2NkJBYUYxV0s5WXhSLlNleUowSWpvaVpHMGlMQ0p6SWpwN0luVnlhU0k2SW1oMGRIQTZMeTh4TUM0eE1DNDFNQzR4TURVNk9EQXdNQzlrYVdSamIyMXRJaXdpY2lJNlcxMHNJbUVpT2xzaVpHbGtZMjl0YlM5Mk1pSmRmWDAiLCJib2R5Ijp7ImFjY2VwdCI6W119fQ==",
//         };
//
//         // Act
//         await handler.Handle(request, CancellationToken.None);
//
//         // Assert
//         _httpClientFactory.LastCreatedClient.DefaultRequestHeaders.Should().NotContainKey("apiKey");
//     }
// }