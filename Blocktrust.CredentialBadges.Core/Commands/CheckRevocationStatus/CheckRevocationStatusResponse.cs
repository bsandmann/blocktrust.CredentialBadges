namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

public class CheckRevocationStatusResponse
{
    // TODO Still a bit unclear
    public bool? Revoked { get; set; }
    public string? ReferenceToRevocationList { get; set; }
}