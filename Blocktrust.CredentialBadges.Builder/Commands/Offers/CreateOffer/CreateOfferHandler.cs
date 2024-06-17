using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class CreateOfferHandler : IRequestHandler<CreateOfferRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CreateOfferHandler> _logger;
 
    public CreateOfferHandler(IHttpClientFactory httpClientFactory, ILogger<CreateOfferHandler> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<Result<string>> Handle(CreateOfferRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Add the apiKey to the request headers
            _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);

            var response = await _httpClient.PostAsJsonAsync("http://212.124.52.36:35412/cloud-agent/issue-credentials/credential-offers", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                
                
                
                return Result.Ok(responseBody);
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result.Fail(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating offer");
            return Result.Fail(ex.Message);
        }
    }
}