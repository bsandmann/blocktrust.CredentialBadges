namespace Blocktrust.CredentialBadges.Core.Commands.CheckTrustRegistry;

public class CheckTrustRegistryResponse
{
    /// <summary>
    /// TODO Still a bit unclear
    /// </summary>
    public bool? IssuerFoundInTrustRegistry { get; set; }
    public string? ReferenceToTrustRegistry { get; set; }
}