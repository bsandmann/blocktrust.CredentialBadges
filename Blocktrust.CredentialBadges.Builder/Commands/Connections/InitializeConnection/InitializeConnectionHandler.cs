using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.Connections;

public class InitializeConnectionHandler : IRequestHandler<InitializeConnectionRequest, Result<InitializeConnectionResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<InitializeConnectionHandler> _logger;

    public InitializeConnectionHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<InitializeConnectionHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("AdminAgent");

        // Set the base URL and header for the HttpClient
        _httpClient.BaseAddress = new Uri(_appSettings.AdminAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.AdminAgentAdminKey);
    }

    public async Task<Result<InitializeConnectionResponse>> Handle(InitializeConnectionRequest request, CancellationToken cancellationToken)
    {
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.AdminAgentBaseUrl 
        };

        try
        {
            var response = await identusClient.CreateConnectionAsync(new CreateConnectionRequest(), cancellationToken);
            var result = new InitializeConnectionResponse
            {
                ConnectionId = response.ConnectionId.ToString(),
                InvitationUrl = response.Invitation.InvitationUrl
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating connection");
            return Result.Fail<InitializeConnectionResponse>(ex.Message);
        }
    }
}