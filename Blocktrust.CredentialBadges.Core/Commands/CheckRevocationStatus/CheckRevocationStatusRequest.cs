using MediatR;
using Blocktrust.CredentialBadges.OpenBadges;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

public class CheckRevocationStatusRequest : IRequest<CheckRevocationStatusResponse>
{
    public CheckRevocationStatusRequest(CredentialStatus credentialStatus)
    {
        CredentialStatus = credentialStatus;
    }

    public CredentialStatus CredentialStatus { get; }
}
