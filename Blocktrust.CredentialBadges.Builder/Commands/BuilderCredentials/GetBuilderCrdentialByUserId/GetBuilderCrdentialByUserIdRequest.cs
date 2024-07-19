using MediatR;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialsByUserId;

public class GetBuilderCredentialsByUserIdRequest : IRequest<Result<List<BuilderCredential>>>
{
    public string UserId { get; set; }
}