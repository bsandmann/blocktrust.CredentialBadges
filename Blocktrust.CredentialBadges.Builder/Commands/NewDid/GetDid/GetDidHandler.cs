using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class GetDidHandler : IRequestHandler<GetDidRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<GetDidHandler> _logger;
    private readonly IdentusClient _identusClient;

    public GetDidHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<GetDidHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");
        _identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.UserAgentBaseUrl // Ensure the base URL is set
        };
    }

    public async Task<Result<string>> Handle(GetDidRequest request, CancellationToken cancellationToken)
    {
        // Set the API key in the request headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);

        try
        {
            var createDidRequest = new CreateManagedDidRequest();

            // createDidRequest.DocumentTemplate.PublicKeys.Add(new ManagedDIDKeyTemplate(
            //     ));
                        
                

            var createDidResponse = await _identusClient.PostDidRegistrarDidsAsync(createDidRequest, cancellationToken);

            if (!string.IsNullOrEmpty(createDidResponse.LongFormDid))
            {
                return Result.Ok(createDidResponse.LongFormDid);
            }

            return Result.Fail<string>("Failed to create DID or empty response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or retrieving DID");
            return Result.Fail<string>(ex.Message);
        }
    }
}