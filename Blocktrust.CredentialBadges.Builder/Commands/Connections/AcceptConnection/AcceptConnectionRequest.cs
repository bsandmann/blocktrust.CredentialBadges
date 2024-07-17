using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Connections;

public class AcceptConnectionRequest : IRequest<Result<AcceptConnectionResponse>>
{
    public string ApiKey { get; set; }
    public string InvitationUrl { get; set; }
}