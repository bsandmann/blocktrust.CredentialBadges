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

    [StringLength(5000, ErrorMessage = "Claims must not exceed 5000 characters")]
    [Column(TypeName = "text")]
    public string? Claims { get; set; }
}