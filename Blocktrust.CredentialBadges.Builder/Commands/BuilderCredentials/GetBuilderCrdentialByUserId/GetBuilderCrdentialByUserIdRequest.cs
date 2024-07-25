namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;

using Blocktrust.CredentialBadges.Builder.Domain;
using FluentResults;
using MediatR;

public class GetBuilderCredentialsByUserIdRequest : IRequest<Result<List<BuilderCredential>>>
{
    public string UserId { get; set; }
}