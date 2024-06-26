using Blocktrust.CredentialBadges.Web.Entities;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerifiedCredential
{
    public Guid Id { get; set; }
    public CredentialStatus Status { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string Credential { get; set; }

    public enum CredentialStatus
    {
        Verified,
        Revoked,
        Expired,
        NotDue
    }

    public VerifiedCredentialEntity ToEntity()
    {
        return new VerifiedCredentialEntity
        {
            StoredCredentialId = Id,
            Status = (VerifiedCredentialEntity.CredentialStatus)Status,
            Description = Description,
            Name = Name,
            Image = Image,
            Credential = Credential
        };
    }

    public static VerifiedCredential FromEntity(VerifiedCredentialEntity entity)
    {
        return new VerifiedCredential
        {
            Id = entity.StoredCredentialId,
            Status = (CredentialStatus)entity.Status,
            Description = entity.Description,
            Name = entity.Name,
            Image = entity.Image,
            Credential = entity.Credential
        };
    }
}