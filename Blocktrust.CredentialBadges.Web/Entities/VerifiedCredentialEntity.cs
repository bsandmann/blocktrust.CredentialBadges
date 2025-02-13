using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Entities;

public class VerifiedCredentialEntity
{
    [Key]
    public Guid StoredCredentialId { get; set; }

    [Required]
    public EVerificationStatus Status { get; set; } = EVerificationStatus.Invalid;

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string Name { get; set; }

    [Column(TypeName = "text")]
    public string? Image { get; set; }

    [Required(ErrorMessage = "Credential is required")]
    [Column(TypeName = "text")]
    public string Credential { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public string Issuer { get; set; }

    public string TemplateId { get; set; } = "noimage_no_description_light";

    [StringLength(253, ErrorMessage = "Domain must not exceed 253 characters")]
    public string? Domain { get; set; }

    // New property to store the serialized claims data
    [StringLength(5000, ErrorMessage = "Claims must not exceed 5000 characters")]
    [Column(TypeName = "text")]
    public string? Claims { get; set; }
}