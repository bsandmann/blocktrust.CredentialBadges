using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class CreateOfferHandler : IRequestHandler<CreateOfferRequest, Result<OfferResponse>>
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

    public async Task<Result<OfferResponse>> Handle(CreateOfferRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.AdminApiKey);
        var identusClient = new IdentusClient(_httpClient);

        identusClient.BaseUrl = _appSettings.AdminAgentBaseUrl;
        
        // Ensure the subject DID is in short form
        request.IssuingDID = GetShortDid(request.IssuingDID);

        try
        {
            var response = await identusClient.CreateCredentialOfferAsync(request, cancellationToken);
            OfferResponse offerResponse = new OfferResponse()
            {
                Success = true,
                RecordId = response.RecordId,
                Thid = response.Thid,
                Message = "Offer created successfully"
            };
                
            
            return Result.Ok(offerResponse);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error creating offer");
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