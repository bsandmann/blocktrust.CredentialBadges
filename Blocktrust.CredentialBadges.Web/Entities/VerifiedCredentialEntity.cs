using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Entities;

public class VerifiedCredentialEntity
{
    [Key]
    public Guid StoredCredentialId { get; set; } // Primary Key


    [Required]
    public EVerificationStatus Status { get; set; } = EVerificationStatus.Invalid;

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string Name { get; set; }

    [Column(TypeName = "text")]
    public string Image { get; set; } = null; // Base64-encoded image string

    [Required(ErrorMessage = "Credential is required")]
    [Column(TypeName = "text")]
    public string Credential { get; set; } // A very long string
    
    public DateTime ValidFrom { get; set; } // Date when the credential is valid from
    
    public DateTime ValidUntil { get; set; } // Date when the credential is valid until
    
    // Issuer
    public string Issuer { get; set; } // The entity that issued the credential
    
    public string TemplateId { get; set; } = "noimage_no_description_light"; // The template ID of the credential
}