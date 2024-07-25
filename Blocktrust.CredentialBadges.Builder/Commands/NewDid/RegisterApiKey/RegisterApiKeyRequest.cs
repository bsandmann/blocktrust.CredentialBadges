namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.RegisterApiKey;

using FluentResults;
using MediatR;

public class RegisterApiKeyRequest : IRequest<Result<string>>
{
    public Guid EntityId { get; set; }
    public string ApiKey { get; set; }
}