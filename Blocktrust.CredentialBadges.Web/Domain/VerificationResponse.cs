using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Web.Enums;

namespace Blocktrust.CredentialBadges.Web.Domain;

public class VerificationResponse
{
    public Guid Id { get; set; }
    public EVerificationStatus Status { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
    public string Issuer { get; set; }
    public string Domain { get; set; }

    public string SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public Dictionary<string,string>? Claims { get; set; }

    public string? Image { get; set; }

    public VerifyOpenBadgeResponse VerificationChecks { get; set; }
}

