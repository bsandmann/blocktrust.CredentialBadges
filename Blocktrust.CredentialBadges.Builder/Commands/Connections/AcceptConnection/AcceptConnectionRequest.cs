namespace Blocktrust.CredentialBadges.Builder.Commands.Connections.AcceptConnection;

using FluentResults;
using MediatR;

public class AcceptConnectionRequest : IRequest<Result<AcceptConnectionResponse>>
{
    public string ApiKey { get; set; }
    public string InvitationUrl { get; set; }
}