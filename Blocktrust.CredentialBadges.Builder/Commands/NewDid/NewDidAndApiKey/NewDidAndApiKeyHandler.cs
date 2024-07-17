using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.Builder.Common;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class NewDidAndApiKeyHandler : IRequestHandler<NewDidAndApiKeyRequest, Result>
{
    private readonly IMediator _mediator;
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<NewDidAndApiKeyHandler> _logger;

    public NewDidAndApiKeyHandler(IMediator mediator, IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<NewDidAndApiKeyHandler> logger)
    {
        _mediator = mediator;
        _httpClient = httpClientFactory.CreateClient("UserAgent");
        _appSettings = appSettings.Value;
        _logger = logger;

        // Set the base URL and header for the HttpClient
        _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
    }

    public async Task<Result> Handle(NewDidAndApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Step 1: Register Wallet
            var registerWalletRequest = new RegisterWalletRequest
            {
                Seed = request.Seed,
                Name = request.WalletName
            };

            var walletResult = await _mediator.Send(registerWalletRequest, cancellationToken);
            if (walletResult.IsFailed)
            {
                return Result.Fail(walletResult.Errors);
            }

            var walletId = walletResult.Value;

            // Step 2: Register Entity with a random short name
            var entityName = GenerateRandomShortName();
            var registerEntityRequest = new RegisterEntityRequest
            {
                Name = entityName,
                WalletId = walletId
            };

            var entityIdResult = await _mediator.Send(registerEntityRequest, cancellationToken);
            if (entityIdResult.IsFailed)
            {
                return Result.Fail(entityIdResult.Errors);
            }

            var entityId = entityIdResult.Value;

            // Step 3: Register API Key
            var apiKey = GenerateRandomApiKey(16);
            var registerApiKeyRequest = new RegisterApiKeyRequest
            {
                EntityId = entityId,
                ApiKey = apiKey
            };

            var apiKeyResult = await _mediator.Send(registerApiKeyRequest, cancellationToken);
            if (apiKeyResult.IsFailed)
            {
                return Result.Fail(apiKeyResult.Errors);
            }

            // Step 4: Get DID using the API key
            var getDidRequest = new GetDidRequest
            {
                ApiKey = apiKeyResult.Value
            };

            var didResult = await _mediator.Send(getDidRequest, cancellationToken);
            if (didResult.IsFailed)
            {
                return Result.Fail(didResult.Errors);
            }

            // Return both did and api key
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling NewDidAndApiKeyRequest");
            return Result.Fail(ex.Message);
        }
    }

    private string GenerateRandomApiKey(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateRandomShortName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8) // Adjust length as needed
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

