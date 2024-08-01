using Blocktrust.CredentialBadges.Builder.Common;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid;

/// <summary>
/// Test implementation of IHttpClientFactory
/// </summary>
public class TestHttpClientFactory : IHttpClientFactory
{
    private readonly IOptions<AppSettings> _appSettings;

    public TestHttpClientFactory(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public HttpClient CreateClient(string name)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_appSettings.Value.UserAgentBaseUrl)
        };
        httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.Value.UserAgentAdminKey);
        return httpClient;
    }
}