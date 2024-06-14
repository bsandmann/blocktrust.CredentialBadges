namespace Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;

using FluentResults;
using MediatR;
using OpenBadges;

public class VerifyOpenBadgeRequest : IRequest<Result<VerifyOpenBadgeResponse>>
{
    public VerifyOpenBadgeRequest(OpenBadgeCredential openBadgeCredential)
    {
        OpenBadgeCredential = openBadgeCredential;
    }

    public OpenBadgeCredential OpenBadgeCredential { get; }
}