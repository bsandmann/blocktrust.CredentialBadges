using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.Builder.Common;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterApiKeyHandler : IRequestHandler<RegisterApiKeyRequest, Result>
{
    private readonly IdentusClient _identusClient;
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient;

    public RegisterApiKeyHandler(IdentusClient identusClient, IOptions<AppSettings> appSettings, HttpClient httpClient)
    {
        _identusClient = identusClient;
        _appSettings = appSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<Result> Handle(RegisterApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Prepare the request body
            var apiKeyRequest = new ApiKeyAuthenticationRequest
            {
                EntityId = request.EntityId,
                ApiKey = request.ApiKey
            };

            // Set headers directly on HttpClient
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
                
            // Call the API method to register the API key for the entity
            await _identusClient.AddEntityApiKeyAuthenticationAsync(apiKeyRequest, cancellationToken);
                
            return Result.Ok();
        }
        catch (ApiException ex)
        {
            return Result.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}