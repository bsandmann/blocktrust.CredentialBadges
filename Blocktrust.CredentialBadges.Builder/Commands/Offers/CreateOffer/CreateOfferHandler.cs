using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class CreateOfferHandler : IRequestHandler<CreateOfferRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CreateOfferHandler> _logger;
    private readonly AppSettings _appSettings;

    public CreateOfferHandler(IHttpClientFactory httpClientFactory, ILogger<CreateOfferHandler> logger, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(CreateOfferRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.Agent1ApiKey);
        var identusClient = new IdentusClient(_httpClient);

        identusClient.BaseUrl = _appSettings.Agent1BaseUrl; // or Agent2BaseUrl 

        try
        {
            var reqs = _httpClient.DefaultRequestHeaders.ToString();
            var response = await identusClient.CreateCredentialOfferAsync(request, cancellationToken);
            return Result.Ok(response.RecordId);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error creating offer");
            return Result.Fail(ex.Message);
        }
    }
}