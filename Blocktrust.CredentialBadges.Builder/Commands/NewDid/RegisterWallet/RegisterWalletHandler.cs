using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterWalletHandler : IRequestHandler<RegisterWalletRequest, Result<Guid>>
{
    private readonly IdentusClient _identusClient;
    private readonly AppSettings _appSettings;
    private readonly HttpClient _httpClient; 

    public RegisterWalletHandler(IdentusClient identusClient, IOptions<AppSettings> appSettings, HttpClient httpClient)
    {
        _identusClient = identusClient;
        _appSettings = appSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<Result<Guid>> Handle(RegisterWalletRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var createWalletRequest = new CreateWalletRequest
            {
                Seed = request.Seed,
                Name = request.Name
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);

            var response = await _identusClient.CreateWalletAsync(createWalletRequest, cancellationToken);
            return Result.Ok(response.Id);
        }
        catch (ApiException ex)
        {
            return Result.Fail<Guid>(ex.Message);
        }
    }
}