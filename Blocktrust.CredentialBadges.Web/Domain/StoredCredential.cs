using Blocktrust.CredentialBadges.Web.Entities;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class StoredCredential
{
    public Guid Id { get; set; }
    public CredentialStatus Status { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public byte[] Image { get; set; }
    public string Credential { get; set; }

    public enum CredentialStatus
    {
        Verified,
        Revoked,
        Expired,
        NotDue
    }

    public StoredCredentialEntity ToEntity()
    {
        return new StoredCredentialEntity
        {
            StoredCredentialId = Id,
            Status = (StoredCredentialEntity.CredentialStatus)Status,
            Description = Description,
            Name = Name,
            Image = Image,
            Credential = Credential
        };
    }

    public static StoredCredential FromEntity(StoredCredentialEntity entity)
    {
        return new StoredCredential
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