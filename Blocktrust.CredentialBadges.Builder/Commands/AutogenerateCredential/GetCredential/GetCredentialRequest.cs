using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.AutogenerateCredential.GetCredential;

public class GetCredentialRequest : IRequest<Result<string>>
{
    public string RecordId { get; }

    public GetCredentialRequest(string recordId)
    {
        RecordId = recordId;
    }
}