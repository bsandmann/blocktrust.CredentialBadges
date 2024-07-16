using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterEntityHandler : IRequestHandler<RegisterEntityRequest, Result<Guid>>
{
    private readonly IdentusClient _identusClient;
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient; // Assuming IdentusClient exposes HttpClient

    public RegisterEntityHandler(IdentusClient identusClient, IOptions<AppSettings> appSettings, HttpClient httpClient)
    {
        _identusClient = identusClient;
        _appSettings = appSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<Result<Guid>> Handle(RegisterEntityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var createEntityRequest = new CreateEntityRequest
            {
                Name = request.Name,
                WalletId = request.WalletId
            };

            // Set headers directly on HttpClient
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.UserAgentAdminKey);

            var response = await _identusClient.CreateEntityAsync(createEntityRequest, cancellationToken);
            return Result.Ok(response.Id);
        }
        catch (ApiException ex)
        {
            return Result.Fail<Guid>(ex.Message);
        }
    }
}