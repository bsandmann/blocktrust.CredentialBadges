using Blocktrust.CredentialBadges.Web.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;

public class StoreVerifiedCredentialRequest : IRequest<Result<VerifiedCredential>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] Image { get; set; }
    public string Credential { get; set; }
    public VerifiedCredential.CredentialStatus Status { get; set; }
}