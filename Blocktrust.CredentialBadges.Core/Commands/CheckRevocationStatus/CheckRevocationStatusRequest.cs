namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

using FluentResults;
using MediatR;
using OpenBadges;

public class CheckRevocationStatusRequest : IRequest<Result<CheckRevocationStatusResponse>>
{
    public CheckRevocationStatusRequest(OpenBadgeCredential openBadgeCredential)
    {
        OpenBadgeCredential = openBadgeCredential;
    }

    public OpenBadgeCredential OpenBadgeCredential { get; } 
}