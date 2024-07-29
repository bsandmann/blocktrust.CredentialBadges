using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterWalletHandler : IRequestHandler<RegisterWalletRequest, Result<Guid>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<RegisterWalletHandler> _logger;

    public RegisterWalletHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<RegisterWalletHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");

        // Set the base URL and header for the HttpClient
        _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
    }

    public async Task<Result<Guid>> Handle(RegisterWalletRequest request, CancellationToken cancellationToken)
    {
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.UserAgentBaseUrl // Ensure the base URL is set
        };

        try
        {
            var createWalletRequest = new CreateWalletRequest
            { 
                Id =  Guid.NewGuid(),
                Seed = request.Seed,
                Name = request.Name
            };

            var response = await identusClient.CreateWalletAsync(createWalletRequest, cancellationToken);
            return Result.Ok(response.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wallet");
            return Result.Fail<Guid>(ex.Message);
            
        }
    }
}