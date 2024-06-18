using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.IdentusClientApi;
using Blocktrust.CredentialBadges.Builder.Common;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.AcceptOffer;

public class AcceptOfferHandler : IRequestHandler<AcceptOfferRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AcceptOfferHandler> _logger;
    private readonly AppSettings _appSettings;

    public AcceptOfferHandler(IHttpClientFactory httpClientFactory, ILogger<AcceptOfferHandler> logger, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(AcceptOfferRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.Agent2ApiKey);
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.Agent2BaseUrl
        };

        try
        {
            var acceptCredentialOfferRequest = new AcceptCredentialOfferRequest
            {
                SubjectId = request.SubjectId
            };

            var response = await identusClient.AcceptCredentialOfferAsync(request.RecordId, acceptCredentialOfferRequest, cancellationToken);
                
            return Result.Ok(response.RecordId);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error accepting offer");
            return Result.Fail(ex.Message);
        }
    }
}