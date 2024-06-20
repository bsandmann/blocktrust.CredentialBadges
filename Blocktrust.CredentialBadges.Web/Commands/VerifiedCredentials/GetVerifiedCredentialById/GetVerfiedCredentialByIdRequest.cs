namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using Domain;
using FluentResults;

public class GetVerifiedCredentialByIdRequest : IRequest<Result<VerifiedCredential>>
{
    public Guid CredentialId { get; set; }

    public GetVerifiedCredentialByIdRequest(Guid credentialId)
    {
        CredentialId = credentialId;
    }
}