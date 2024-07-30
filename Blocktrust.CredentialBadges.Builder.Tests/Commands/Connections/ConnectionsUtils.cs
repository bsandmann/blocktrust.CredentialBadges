using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blocktrust.CredentialBadges.Builder.Commands.Connections;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;


namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.Connections;

public class TestHttpClientFactory : IHttpClientFactory
{
    private readonly IOptions<AppSettings> _appSettings;
    public HttpClient LastCreatedClient { get; private set; }
    public bool ShouldThrowException { get; set; }

    public TestHttpClientFactory(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public HttpClient CreateClient(string name)
    {
        var client = new HttpClient(new TestHttpMessageHandler(ShouldThrowException));
        client.BaseAddress = new Uri(_appSettings.Value.AdminAgentBaseUrl);
        client.DefaultRequestHeaders.Add("apiKey", _appSettings.Value.AdminApiKey);
        LastCreatedClient = client;
        return client;
    }
}

public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly bool _shouldThrowException;

    public TestHttpMessageHandler(bool shouldThrowException)
    {
        _shouldThrowException = shouldThrowException;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_shouldThrowException)
        {
            throw new Exception("Test exception");
        }

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"connectionId\":\"test-connection-id\",\"invitation\":{\"invitationUrl\":\"https://example.com/invitation\"}}")
        };
        return Task.FromResult(response);
    }
}
