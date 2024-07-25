namespace Blocktrust.CredentialBadges.Builder.Commands.Connections.InitializeConnection;

using FluentResults;
using MediatR;

public class InitializeConnectionRequest : IRequest<Result<InitializeConnectionResponse>> { }