using Blocktrust.CredentialBadges.Builder.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blocktrust.CredentialBadges.Builder.Data.Entities;

public class BuilderCredentialEntity
{
    [Key]
    public Guid CredentialId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(100)]
    public string Label { get; set; }

    [Required]
    [MaxLength(100)]
    public string SubjectDid { get; set; }

    [Required]
    [MaxLength(100)]
    public string IssuerDid { get; set; }

    [Required]
    public EBuilderCredentialStatus Status { get; set; }

    [Required]
    public Guid IssuerConnectionId { get; set; }

    [Required]
    public Guid SubjectConnectionId { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string CredentialSubject { get; set; }
}