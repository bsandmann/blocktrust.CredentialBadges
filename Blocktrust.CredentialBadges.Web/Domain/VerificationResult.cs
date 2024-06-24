using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerificationResult
{
    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }
}