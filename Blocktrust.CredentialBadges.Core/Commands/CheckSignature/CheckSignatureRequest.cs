namespace Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

using Blocktrust.CredentialBadges.OpenBadges;
using CheckTrustRegistry;
using FluentResults;
using MediatR;

public class CheckSignatureRequest : IRequest<Result<ECheckSignatureResponse>>
{
    public CheckSignatureRequest(OpenBadgeCredential openBadgeCredential)
    {
        OpenBadgeCredential = openBadgeCredential;
    }

    public OpenBadgeCredential OpenBadgeCredential { get; }
}