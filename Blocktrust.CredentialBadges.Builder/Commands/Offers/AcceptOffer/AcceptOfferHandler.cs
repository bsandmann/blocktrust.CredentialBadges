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
        // Ensure the subject DID is in short form
        request.SubjectId = GetShortDid(request.SubjectId);

        // if api key is null, use agent 2 api key
        if (string.IsNullOrEmpty(request.ApiKey))
            _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.SubjectApiKey);
        else
        {
            _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);
        }

        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.Agent2BaseUrl
        };

        try
        {
            AcceptCredentialOfferRequest acceptCredentialOfferRequest = new AcceptCredentialOfferRequest
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

    private string GetShortDid(string did)
    {
        // Split the DID by colons
        var parts = did.Split(':');

        // Check if the DID has more than three parts (indicating long form)
        if (parts.Length > 3)
        {
            // Return the first three parts joined by colons
            return string.Join(":", parts[0], parts[1], parts[2]);
        }
        else
        {
            // Return the DID itself if it is already in short form
            return did;
        }
    }
}