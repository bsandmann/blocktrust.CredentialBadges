using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Enums;

namespace Blocktrust.CredentialBadges.Builder.Domain;

public class BuilderCredential
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
    
    public string UserId { get; set; }


    public static BuilderCredential FromEntity(BuilderCredentialEntity entity)
    {
        return new BuilderCredential
        {
            CredentialId = entity.CredentialId,
            Date = entity.Date,
            Label = entity.Label,
            SubjectDid = entity.SubjectDid,
            IssuerDid = entity.IssuerDid,
            Status = entity.Status,
            IssuerConnectionId = entity.IssuerConnectionId,
            SubjectConnectionId = entity.SubjectConnectionId,
            CredentialSubject = entity.CredentialSubject,
            UserId = entity.UserId
            
        };
    }

    public BuilderCredentialEntity ToEntity()
    {
        return new BuilderCredentialEntity
        {
            CredentialId = CredentialId,
            Date = Date,
            Label = Label,
            SubjectDid = SubjectDid,
            IssuerDid = IssuerDid,
            Status = Status,
            IssuerConnectionId = IssuerConnectionId,
            SubjectConnectionId = SubjectConnectionId,
            CredentialSubject = CredentialSubject,
            UserId = UserId
        };
    }
}