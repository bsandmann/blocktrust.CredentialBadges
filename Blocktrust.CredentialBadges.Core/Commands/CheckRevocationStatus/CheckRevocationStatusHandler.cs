namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

using FluentResults;
using MediatR;

public class CheckRevocationStatusHandler : IRequestHandler<CheckRevocationStatusRequest,Result<CheckRevocationStatusResponse>>
{
    public async Task<Result<CheckRevocationStatusResponse>> Handle(CheckRevocationStatusRequest request, CancellationToken cancellationToken)
    {
        // inject HttpClient and do the checks
        
        return  Result.Ok(new CheckRevocationStatusResponse()
        {
            Revoked = false,
            ReferenceToRevocationList = "https://???"
        });
    }
}