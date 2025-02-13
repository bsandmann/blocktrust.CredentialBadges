namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using Domain;
using FluentResults;
using System;

public class GetVerifiedCredentialByIdRequest : IRequest<Result<VerifiedCredential>>
{
    public Guid CredentialId { get; }
    public bool SkipCache { get; }

    public GetVerifiedCredentialByIdRequest(Guid credentialId, bool skipCache = false)
    {
        CredentialId = credentialId;
        SkipCache = skipCache;
    }
}