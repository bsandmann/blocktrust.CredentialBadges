using System.Net;
using Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetRecordById;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.Credentials.GetRecordByIdHandler;
/// <summary>
///  Test for fetching credential record by id
/// </summary>
public class GetRecordByIdHandlerTests
{
    private readonly IOptions<AppSettings> _appSettings;
    private readonly ILogger<Builder.Commands.Credentials.GetRecordById.GetRecordByIdHandler> _logger;
    private readonly TestHttpClientFactory _httpClientFactory;
    /// <summary>
    ///   Constructor to initialize the test class
    /// </summary>
    public GetRecordByIdHandlerTests()
    {
        // Setup service collection
        var services = new ServiceCollection();
        services.AddLogging();
            
        // Configure AppSettings directly
        services.Configure<AppSettings>(options =>
        {
            options.UserApiKey = "597634cfaa4feae5";
            options.UserAgentBaseUrl = "http://212.124.52.36:35413/cloud-agent/";
        });

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get services
        _appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        _logger = serviceProvider.GetRequiredService<ILogger<Builder.Commands.Credentials.GetRecordById.GetRecordByIdHandler>>();

        // Initialize HttpClientFactory
        _httpClientFactory = new TestHttpClientFactory(_appSettings);
    }
    /// <summary>
    ///  Test to get credential record by id when record exists
    /// </summary>
    
    // [Fact]
    // public async Task Handle_WithValidRequest_ShouldReturnSuccessResult()
    // {
    //     // Arrange
    //     var handler = new Builder.Commands.Credentials.GetRecordById.GetRecordByIdHandler(_httpClientFactory, _logger, _appSettings);
    //     var request = new GetRecordByIdRequest("99fd9527-9a64-43ed-ada7-a8930dda1a5f", "PZkVcZSzhVh3Q2Pm");
    //
    //     // Act
    //     var result = await handler.Handle(request, CancellationToken.None);
    //
    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    //     result.Value.Should().NotBeNull();
    //     result.Value.Should().BeOfType<IssueCredentialRecord>();
    // }
    
    /// <summary>
    ///  Test to get credential record by id when record does not exist
    /// </summary>

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldReturnFailResult()
    {
        // Arrange
        var handler = new Builder.Commands.Credentials.GetRecordById.GetRecordByIdHandler(_httpClientFactory, _logger, _appSettings);
        var request = new GetRecordByIdRequest("99fd9527-9a64-43ed-ada7-a8930dda1aj9");

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("Error retrieving credential record");
    }

    /// <summary>
    ///  Test to get credential record by id when record exists
    /// </summary>
    [Fact]
    public async Task Handle_WithCustomApiKey_ShouldUseProvidedApiKey()
    {
        // Arrange
        var handler = new Builder.Commands.Credentials.GetRecordById.GetRecordByIdHandler(_httpClientFactory, _logger, _appSettings);
        var request = new GetRecordByIdRequest("valid-record-id", "custom-api-key");

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<IssueCredentialRecord>();
        _httpClientFactory.LastCreatedClient.DefaultRequestHeaders.GetValues("apiKey").Should().ContainSingle().Which.Should().Be("custom-api-key");
    }
}

/// <summary>
///  Test HttpClientFactory
/// </summary>
public class TestHttpClientFactory : IHttpClientFactory
{
    private readonly IOptions<AppSettings> _appSettings;
    public HttpClient LastCreatedClient { get; private set; }

    public TestHttpClientFactory(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public HttpClient CreateClient(string name)
    {
        var client = new HttpClient(new TestHttpMessageHandler());
        client.BaseAddress = new Uri(_appSettings.Value.UserAgentBaseUrl);
        LastCreatedClient = client;
        return client;
    }
}
/// <summary>
///  Test HttpMessageHandler
/// </summary>
public class TestHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri.PathAndQuery.Contains("valid-record-id"))
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":\"valid-record-id\",\"state\":\"issued\"}")
            };
            return Task.FromResult(response);
        }
        else
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}