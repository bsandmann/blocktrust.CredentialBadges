using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class NewDidAndApiKeyHandler : IRequestHandler<NewDidAndApiKeyRequest, Result<DidAndApiKeyResponse>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NewDidAndApiKeyHandler> _logger;

    public NewDidAndApiKeyHandler(IMediator mediator, ILogger<NewDidAndApiKeyHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<DidAndApiKeyResponse>> Handle(NewDidAndApiKeyRequest request, CancellationToken cancellationToken)
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
                return Result.Fail<DidAndApiKeyResponse>(walletResult.Errors);
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
                return Result.Fail<DidAndApiKeyResponse>(entityIdResult.Errors);
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
                return Result.Fail<DidAndApiKeyResponse>(apiKeyResult.Errors);
            }

            // Step 4: Get DID using the API key via another mediator request
            var getDidRequest = new GetDidRequest
            {
                ApiKey = apiKey
            };

            var didResult = await _mediator.Send(getDidRequest, cancellationToken);
            if (didResult.IsFailed)
            {
                return Result.Fail<DidAndApiKeyResponse>(didResult.Errors);
            }

            return Result.Ok(new DidAndApiKeyResponse
            {
                Did = didResult.Value,
                ApiKey = apiKey
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling NewDidAndApiKeyRequest");
            return Result.Fail<DidAndApiKeyResponse>(ex.Message);
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