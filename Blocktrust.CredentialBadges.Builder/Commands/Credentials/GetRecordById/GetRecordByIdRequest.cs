using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Credentials.GetRecordById;

public class GetRecordByIdRequest : IRequest<Result<IssueCredentialRecord>>
{
    public string RecordId { get; set; }
    public string ApiKey { get; set; }

    public GetRecordByIdRequest(string recordId)
    {
        RecordId = recordId;
        
    }

    public GetRecordByIdRequest(string recordId, string apiKey)
    {
        RecordId = recordId;
        ApiKey = apiKey;

    }
}