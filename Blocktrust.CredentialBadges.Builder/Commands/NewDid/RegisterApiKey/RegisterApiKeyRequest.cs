using Blocktrust.CredentialBadges.OpenBadges;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterApiKeyRequest : IRequest<Result>, IRequest<FluentResults.Result>
{
    public Guid EntityId { get; set; }
    public string ApiKey { get; set; }
}