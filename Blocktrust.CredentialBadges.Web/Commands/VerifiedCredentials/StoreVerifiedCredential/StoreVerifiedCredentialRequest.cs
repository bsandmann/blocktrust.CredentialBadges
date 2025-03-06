using Blocktrust.CredentialBadges.Web.Domain;
using Blocktrust.CredentialBadges.Web.Enums;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;

public class StoreVerifiedCredentialRequest : IRequest<Result<VerifiedCredential>>
{
    public string Image { get; set; }
    public string Credential { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public string Issuer { get; set; }
    public EVerificationStatus Status { get; set; }
    public string? Domain { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
}