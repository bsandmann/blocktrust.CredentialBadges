using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerificationResponse
{
    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string? Image { get; set; }
}