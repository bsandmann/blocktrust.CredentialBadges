using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.AutogenerateCredential.GenerateCredential;

public class GenerateCredentialRequest : CreateIssueCredentialRecordRequest, IRequest<Result<string>>
{
    public string SubjectId { get; set; }
}