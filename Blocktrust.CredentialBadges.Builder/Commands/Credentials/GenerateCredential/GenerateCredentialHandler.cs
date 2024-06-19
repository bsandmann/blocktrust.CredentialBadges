using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;
using Blocktrust.CredentialBadges.IdentusClientApi;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.AcceptOffer;
using Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetCredential;

namespace Blocktrust.CredentialBadges.Builder.Commands.Credentials.GenerateCredential;

public class GenerateCredentialHandler : IRequestHandler<GenerateCredentialRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenerateCredentialHandler> _logger;
    private readonly AppSettings _appSettings;
    private readonly IMediator _mediator;

    public GenerateCredentialHandler(IHttpClientFactory httpClientFactory, ILogger<GenerateCredentialHandler> logger, IOptions<AppSettings> appSettings, IMediator mediator)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Result<string>> Handle(GenerateCredentialRequest request, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("apiKey", _appSettings.Agent1ApiKey);
        // Create offer using the request directly
        var createOfferResponse = await _mediator.Send(new CreateOfferRequest
        {
            IssuingDID = request.IssuingDID,
            ConnectionId = request.ConnectionId,
            AutomaticIssuance = request.AutomaticIssuance,
            CredentialFormat = request.CredentialFormat,
            Claims = request.Claims
        }, cancellationToken);

        if (createOfferResponse.IsFailed)
        {
            _logger.LogError("Failed to create offer: {Message}", createOfferResponse.Errors[0].Message);
            return Result.Fail(createOfferResponse.Errors);
        }

        var thid = createOfferResponse.Value.Thid;

        // Wait for 9 seconds
        await Task.Delay(9000, cancellationToken);

        // Retry logic for retrieving offer by thid
        const int maxRetries = 3;
        const int delayBetweenRetries = 6000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var getOfferRequest = new GetOfferByThIdRequest(thid);
            var getOfferResponse = await _mediator.Send(getOfferRequest, cancellationToken);

            if (getOfferResponse.IsSuccess)
            {
                var recordId = getOfferResponse.Value;

                // Accept the offer
                var acceptOfferResponse = await _mediator.Send(new AcceptOfferRequest(recordId, request.SubjectId), cancellationToken);

                if (acceptOfferResponse.IsFailed)
                {
                    _logger.LogError("Failed to accept offer: {Message}", acceptOfferResponse.Errors[0].Message);
                    return Result.Fail(acceptOfferResponse.Errors);
                }

                var acceptedRecordId = acceptOfferResponse.Value;

                // Get the credential
                var getCredentialResponse = await _mediator.Send(new GetCredentialRequest(acceptedRecordId), cancellationToken);

                if (getCredentialResponse.IsSuccess)
                {
                    return Result.Ok(getCredentialResponse.Value);
                }

                _logger.LogError("Failed to retrieve credential: {Message}", getCredentialResponse.Errors[0].Message);
                return Result.Fail(getCredentialResponse.Errors);
            }

            if (attempt < maxRetries)
            {
                _logger.LogWarning("Attempt {Attempt} to retrieve offer failed. Retrying in {DelayBetweenRetries} seconds...", attempt, delayBetweenRetries / 1000);
                await Task.Delay(delayBetweenRetries, cancellationToken);
            }
            else
            {
                _logger.LogError("All attempts to retrieve offer failed.");
            }
        }

        return Result.Fail("Failed to retrieve offer after multiple attempts.");
    }
}