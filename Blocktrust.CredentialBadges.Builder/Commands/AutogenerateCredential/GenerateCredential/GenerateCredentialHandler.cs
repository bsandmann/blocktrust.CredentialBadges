using Blocktrust.CredentialBadges.Builder.Commands.AutogenerateCredential.GetCredential;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.AcceptOffer;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;
using Blocktrust.CredentialBadges.Builder.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blocktrust.CredentialBadges.Builder.Commands.AutogenerateCredential.GenerateCredential;

public class GenerateCredentialHandler : IRequestHandler<GenerateCredentialRequest, Result<string>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly IMediator _mediator;

    public GenerateCredentialHandler(IHttpClientFactory httpClientFactory, ILogger<GenerateCredentialHandler> logger, IOptions<AppSettings> appSettings, IMediator mediator)
    {
        _httpClient = httpClientFactory.CreateClient("IdentusAgents");
        _appSettings = appSettings.Value;
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
            Claims = request.Claims,
            ValidityPeriod = request.ValidityPeriod,
        }, cancellationToken);

        if (createOfferResponse.IsFailed)
        {
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
                var recordId = getOfferResponse.Value.RecordId;

                // Accept the offer
                var acceptOfferResponse = await _mediator.Send(new AcceptOfferRequest(recordId, request.SubjectId), cancellationToken);

                if (acceptOfferResponse.IsFailed)
                {
                    return Result.Fail(acceptOfferResponse.Errors);
                }

                var acceptedRecordId = acceptOfferResponse.Value;

                // Get the credential
                var getCredentialResponse = await _mediator.Send(new GetCredentialRequest(acceptedRecordId), cancellationToken);

                if (getCredentialResponse.IsSuccess)
                {
                    return Result.Ok(getCredentialResponse.Value);
                }

                return Result.Fail(getCredentialResponse.Errors);
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(delayBetweenRetries, cancellationToken);
            }
     
        }

        return Result.Fail("Failed to retrieve offer after multiple attempts.");
    }
}