using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.IdentusClientApi;
using Blocktrust.CredentialBadges.Builder.Common;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;

public class GetOfferByThIdHandler : IRequestHandler<GetOfferByThIdRequest, Result<IssueCredentialRecord>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetOfferByThIdHandler> _logger;
    private readonly AppSettings _appSettings;

    public GetOfferByThIdHandler(IHttpClientFactory httpClientFactory, ILogger<GetOfferByThIdHandler> logger, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<Result<IssueCredentialRecord>> Handle(GetOfferByThIdRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.Agent2ApiKey);
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.Agent2BaseUrl
        };

        try
        {
            var response = await identusClient.GetCredentialRecordsAsync(0, 100000, request.ThId, cancellationToken: cancellationToken);

            if (response.Contents.Count > 0)
            {
                var lastOffer = response.Contents.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                return Result.Ok(lastOffer);
            }

            return Result.Fail("No offer found");

        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error retrieving offer by thId");
            return Result.Fail(ex.Message);
        }
    }
}