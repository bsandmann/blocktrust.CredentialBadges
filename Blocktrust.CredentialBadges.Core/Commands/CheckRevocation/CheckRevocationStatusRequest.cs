namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocation;

using MediatR;

public class CheckRevocationStatusRequest : IRequest<CheckRevocationStatusResponse>
{
    public CheckRevocationStatusRequest(CredentialStatus credentialStatus)
    {
        CredentialStatus = credentialStatus;
    }

    public CredentialStatus CredentialStatus { get; }
}

public class CredentialStatus
{
    public string StatusPurpose { get; set; }
    public int StatusListIndex { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
    public string StatusListCredential { get; set; }
}