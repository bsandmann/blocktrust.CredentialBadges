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
    public object ValidFrom { get; set; }
    
    public object ValidUntil { get; set; }
    
    //Isser
    public string Issuer { get; set; }

    
    

   

    public VerifiedCredentialEntity ToEntity()
    {
        return new VerifiedCredentialEntity
        {
            StoredCredentialId = Id,
            Status = (EVerificationStatus)Status,
            Description = Description,
            Name = Name,
            Image = Image,
            Credential = Credential,
            ValidFrom = ValidFrom,
            ValidUntil = ValidUntil,
            Issuer = Issuer
            
            
        };
    }

    public static VerifiedCredential FromEntity(VerifiedCredentialEntity entity)
    {
        return new VerifiedCredential
        {
            Id = entity.StoredCredentialId,
            Status = (EVerificationStatus)entity.Status,
            Description = entity.Description,
            Name = entity.Name,
            Image = entity.Image,
            Credential = entity.Credential,
            ValidFrom = entity.ValidFrom,
            ValidUntil = entity.ValidUntil,
            Issuer = entity.Issuer
        };
    }
}