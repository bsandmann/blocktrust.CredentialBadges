using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerifiedCredential
{
    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string Credential { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public string Issuer { get; set; }
    public string TemplateId { get; set; }
    public string Domain { get; set; }

    public VerifiedCredentialEntity ToEntity()
    {
        return new VerifiedCredentialEntity
        {
            StoredCredentialId = Id,
            Status = Status,
            Description = Description,
            Name = Name,
            Image = Image,
            Credential = Credential,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Issuer = Issuer,
            TemplateId = TemplateId,
            Domain = Domain
        };
    }

    public static VerifiedCredential FromEntity(VerifiedCredentialEntity entity)
    {
        return new VerifiedCredential
        {
            Id = entity.StoredCredentialId,
            Status = entity.Status,
            Description = entity.Description,
            Name = entity.Name,
            Image = entity.Image,
            Credential = entity.Credential,
            ValidFrom = entity.ValidFrom,
            ValidUntil = entity.ValidUntil,
            Issuer = entity.Issuer,
            TemplateId = entity.TemplateId,
            Domain = entity.Domain
        };
    }
}