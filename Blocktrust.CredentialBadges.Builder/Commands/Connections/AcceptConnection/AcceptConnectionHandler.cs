using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.Connections;

public class AcceptConnectionHandler : IRequestHandler<AcceptConnectionRequest, Result<AcceptConnectionResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AcceptConnectionHandler> _logger;

    public AcceptConnectionHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<AcceptConnectionHandler> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");
        _httpClient.BaseAddress = new Uri(appSettings.Value.UserAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public async Task<Result<AcceptConnectionResponse>> Handle(AcceptConnectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var identusClient = new IdentusClient(_httpClient)
            {
                BaseUrl = _httpClient.BaseAddress.ToString()
            };

            if (!string.IsNullOrEmpty(request.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Remove("apiKey");
                _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);
            }

            var acceptInvitationRequest = new AcceptConnectionInvitationRequest
            {
                Invitation = request.InvitationUrl
            };

            var response = await identusClient.AcceptConnectionInvitationAsync(acceptInvitationRequest, cancellationToken);
            var result = new AcceptConnectionResponse
            {
                ConnectionId = response.ConnectionId.ToString()
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting connection");
            return Result.Fail<AcceptConnectionResponse>(ex.Message);
        }
    }
}