using Blocktrust.CredentialBadges.Builder.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class GetDidHandler : IRequestHandler<GetDidRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<GetDidHandler> _logger;

    public GetDidHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<GetDidHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");
        _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
    }

    public async Task<Result<string>> Handle(GetDidRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apiKey", request.ApiKey);

        try
        { var createDidRequest = new
            {
                documentTemplate = new
                {
                    publicKeys = new[]
                    {
                        new
                        {
                            id = "my-issuing-key",
                            purpose = "assertionMethod"
                        }
                    },
                    services = Array.Empty<object>()
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(createDidRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("did-registrar/dids", jsonContent, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var createDidResponse = JsonSerializer.Deserialize<CreateManagedDidResponse>(responseContent);

            if (createDidResponse != null && !string.IsNullOrEmpty(createDidResponse.LongFormDid))
            {
                return Result.Ok(createDidResponse.LongFormDid);
            }
            else
            {
                return Result.Fail<string>("Failed to create DID or empty response.");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while creating or retrieving DID");
            return Result.Fail<string>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or retrieving DID");
            return Result.Fail<string>(ex.Message);
        }
    }

    private class CreateManagedDidResponse
    {
        [JsonPropertyName("longFormDid")]
        public string LongFormDid { get; set; }
    }
}