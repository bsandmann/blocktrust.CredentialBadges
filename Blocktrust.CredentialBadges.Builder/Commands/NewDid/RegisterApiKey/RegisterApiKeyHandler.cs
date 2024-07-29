using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterApiKeyHandler : IRequestHandler<RegisterApiKeyRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<RegisterApiKeyHandler> _logger;
    
    

    public RegisterApiKeyHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<RegisterApiKeyHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");

        // Set the base URL and header for the HttpClient
        _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
    }

    public async Task<Result<string>> Handle(RegisterApiKeyRequest request, CancellationToken cancellationToken)
    {
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.UserAgentBaseUrl // Ensure the base URL is set
        };

        try
        {
            var apiKeyRequest = new ApiKeyAuthenticationRequest
            {
                EntityId = request.EntityId,
                ApiKey = request.ApiKey
            };

            await identusClient.AddEntityApiKeyAuthenticationAsync(apiKeyRequest, cancellationToken);

            return Result.Ok("API key registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering API key");
            return Result.Fail<string>(ex.Message);
        }
    }
}