using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blocktrust.CredentialBadges.Web.Entities;

public class VerifiedCredentialEntity
{
    [Key]
    public Guid StoredCredentialId { get; set; } // Primary Key

    public enum CredentialStatus
    {
        Verified,
        Revoked,
        Expired,
        NotDue
    }

    [Required]
    public CredentialStatus Status { get; set; } = CredentialStatus.Verified;

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
}