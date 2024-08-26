using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.OpenBadges;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;

public class CheckPrismDIDSignatureRequest : IRequest<Result<ECheckSignatureResponse>>
{
    public CheckPrismDIDSignatureRequest(string did, OpenBadgeCredential openBadgeCredential)
    {
        Did = did;
        OpenBadgeCredential = openBadgeCredential;
    }

    public string Did { get; }
    public OpenBadgeCredential OpenBadgeCredential { get; }
}