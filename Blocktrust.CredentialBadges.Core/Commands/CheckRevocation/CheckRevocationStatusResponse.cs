namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocation;

public class CheckRevocationStatusResponse
{
    public bool IsRevoked { get; set; }
    public string CredentialId { get; set; }
}