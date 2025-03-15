using Blocktrust.CredentialBadges.Builder.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetRecordById;

public class GetRecordByIdHandler : IRequestHandler<GetRecordByIdRequest, Result<IssueCredentialRecord>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;

    public GetRecordByIdHandler(IHttpClientFactory httpClientFactory, ILogger<GetRecordByIdHandler> logger, IOptions<AppSettings> appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
    }

    public async Task<Result<IssueCredentialRecord>> Handle(GetRecordByIdRequest request, CancellationToken cancellationToken)
    {
      
        if (!string.IsNullOrEmpty(request.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.UserApiKey);
        }
        
        
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.UserAgentBaseUrl
        };

        try
        {
            var response = await identusClient.GetCredentialRecordAsync(request.RecordId, cancellationToken);
            
            Console.WriteLine(response);
            return Result.Ok(response);
        }
        catch (ApiException ex)
        {
            return Result.Fail($"Error retrieving credential record: {ex.Message}");
        }
    }
}

