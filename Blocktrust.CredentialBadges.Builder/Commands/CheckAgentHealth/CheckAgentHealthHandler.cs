using System.Net.Http.Headers;

namespace Blocktrust.CredentialBadges.Builder.Commands.CheckAgentHealth;

using Common;
using FluentResults;
using IdentusClientApi;
using MediatR;
using Microsoft.Extensions.Options;

public class CheckAgentHealthHandler : IRequestHandler<CheckAgentHealthRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CheckAgentHealthHandler> _logger;
    private readonly AppSettings _appSettings;

    public CheckAgentHealthHandler(IHttpClientFactory httpClientFactory, ILogger<CheckAgentHealthHandler> logger, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(CheckAgentHealthRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "some key");
        var identusClient = new IdentusClient(_httpClient);
        if (request.AgentNumber == 1)
        {
            identusClient.BaseUrl = _appSettings.Agent1BaseUrl;
        }
        else
        {
            identusClient.BaseUrl = _appSettings.Agent2BaseUrl;
        }

        try
        {
            var result = await identusClient.SystemHealthAsync(cancellationToken);
            return Result.Ok($"Version: {result.Version}");
        }
        catch (ApiException e)
        {
            return Result.Fail(e.Message);
        }
    }
}