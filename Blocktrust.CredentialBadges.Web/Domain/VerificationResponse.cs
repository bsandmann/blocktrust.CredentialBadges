using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerificationResponse
{
    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string? Image { get; set; }
    
    public VerificationChecks VerificationChecks { get; set; }
}

public class VerificationChecks
{
    public bool SignatureIsValid { get; set; }
    public bool? CredentialIsNotRevoked { get; set; }
    public bool? CredentialIsNotExpired { get; set; }
    public bool? CredentialIssuanceDateIsNotInFuture { get; set; }
}