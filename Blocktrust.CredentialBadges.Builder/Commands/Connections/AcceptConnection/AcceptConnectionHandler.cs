using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blocktrust.CredentialBadges.Builder.Commands.Connections
{
    public class AcceptConnectionHandler : IRequestHandler<AcceptConnectionRequest, Result<AcceptConnectionResponse>>
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AcceptConnectionHandler> _logger;

        public AcceptConnectionHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<AcceptConnectionHandler> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("UserAgent");

            // Set the base URL and header for the HttpClient
            _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
        }

        public async Task<Result<AcceptConnectionResponse>> Handle(AcceptConnectionRequest request, CancellationToken cancellationToken)
        {
            var identusClient = new IdentusClient(_httpClient)
            {
                BaseUrl = _appSettings.UserAgentBaseUrl // Ensure the base URL is set
            };

            try
            {
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
}
