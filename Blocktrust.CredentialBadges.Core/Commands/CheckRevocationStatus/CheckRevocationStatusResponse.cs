namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

public class CheckRevocationStatusResponse
{
    public bool IsRevoked { get; set; }
    public string CredentialId { get; set; }
}