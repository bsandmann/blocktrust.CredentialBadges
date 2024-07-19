using MediatR;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialById;

public class GetBuilderCredentialByIdRequest : IRequest<Result<BuilderCredential>>
{
    public Guid CredentialId { get; set; }

    public GetBuilderCredentialByIdRequest(Guid credentialId)
    {
        CredentialId = credentialId;
    }
}