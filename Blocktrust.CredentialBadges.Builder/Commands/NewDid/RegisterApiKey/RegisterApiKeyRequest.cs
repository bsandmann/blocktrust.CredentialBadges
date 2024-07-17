using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterApiKeyRequest : IRequest<Result<string>>
{
    public Guid EntityId { get; set; }
    public string ApiKey { get; set; }
}