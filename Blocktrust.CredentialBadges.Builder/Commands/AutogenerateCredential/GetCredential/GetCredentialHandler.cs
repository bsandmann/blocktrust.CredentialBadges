using Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetRecordById;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.AutogenerateCredential.GetCredential;

public class GetCredentialHandler : IRequestHandler<GetCredentialRequest, Result<string>>
{
    private readonly IMediator _mediator;

    public GetCredentialHandler(IMediator mediator, ILogger<GetCredentialHandler> logger)
    {
        _mediator = mediator;
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
                    return Result.Fail<string>($"An error occurred while processing the request: {ex.Message}");
                }

                // Wait before making the next attempt
                await Task.Delay(1000, cancellationToken); 
            }

            // If we reach here, the operation did not succeed within the allowed time
            return Result.Fail<string>("Credential not received within the timeout period.");
        }
        catch (Exception ex)
        {
            return Result.Fail<string>("An error occurred while processing the request.");
        }
    }
}