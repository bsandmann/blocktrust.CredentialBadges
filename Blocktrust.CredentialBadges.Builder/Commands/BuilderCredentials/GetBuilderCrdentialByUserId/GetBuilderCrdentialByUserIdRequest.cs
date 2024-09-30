namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;

using Domain;
using FluentResults;
using MediatR;

public class GetBuilderCredentialsByUserIdRequest : IRequest<Result<List<BuilderCredential>>>
{
    public string UserId { get; set; }
    public string SubjectDid { get; set; }

    public GetBuilderCredentialsByUserIdRequest(string userId)
    {
        UserId = userId;
    }

    public GetBuilderCredentialsByUserIdRequest(string userId, string subjectDid)
    {
        UserId = userId;
        SubjectDid = subjectDid;
    }
}