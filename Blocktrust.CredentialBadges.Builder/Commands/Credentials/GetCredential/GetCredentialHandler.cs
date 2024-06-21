using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetRecordById;


namespace Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetCredential;

public class GetCredentialHandler : IRequestHandler<GetCredentialRequest, Result<string>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetCredentialHandler> _logger;

    public GetCredentialHandler(IMediator mediator, ILogger<GetCredentialHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Attempt to get the credential up to a maximum of 45 seconds
            DateTime startTime = DateTime.UtcNow;
            TimeSpan maxDuration = TimeSpan.FromSeconds(45);

            while (DateTime.UtcNow - startTime < maxDuration)
            {
                try
                {
                    // Get record details by sending GetRecordByIdRequest
                    var recordResult = await _mediator.Send(new GetRecordByIdRequest(request.RecordId), cancellationToken);

                    if (recordResult.IsFailed)
                    {
                        _logger.LogError("Failed to retrieve record details: " + recordResult.Errors.First().Message);
                        continue; // Retry if fetching record details fails
                    }

                    var record = recordResult.Value;

                    // Check if ProtocolState is "CredentialReceived"
                    if (record.ProtocolState.ToString() == "CredentialReceived")
                    {
                        return Result.Ok(record.Credential);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing GetCredentialRequest");
                }

                // Wait before making the next attempt
                await Task.Delay(1000, cancellationToken); 
            }

            // If we reach here, the operation did not succeed within the allowed time
            return Result.Fail<string>("Credential not received within the timeout period.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetCredentialRequest");
            return Result.Fail<string>("An error occurred while processing the request.");
        }
    }
}