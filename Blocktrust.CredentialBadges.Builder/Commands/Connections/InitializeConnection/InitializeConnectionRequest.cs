using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Connections;

public class InitializeConnectionRequest : IRequest<Result<InitializeConnectionResponse>> { }