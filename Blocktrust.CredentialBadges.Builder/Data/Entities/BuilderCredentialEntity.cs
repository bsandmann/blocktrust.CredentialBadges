using Blocktrust.CredentialBadges.Builder.Enums;

namespace Blocktrust.CredentialBadges.Builder.Data.Entities;

public class BuilderCredentialEntity
{
    public Guid CredentialId { get; set; }
    public DateTime Date { get; set; }
    public string Label { get; set; }
    public string SubjectDid { get; set; }
    public string IssuerDid { get; set; }
    public EBuilderCredentialStatus Status { get; set; }
    public Guid IssuerConnectionId { get; set; }
    public Guid SubjectConnectionId { get; set; }
    public string CredentialSubject { get; set; }
}