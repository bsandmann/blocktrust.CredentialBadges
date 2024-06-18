using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.AcceptOffer;

public class AcceptOfferRequest : IRequest<Result<string>>
{
    public string RecordId { get; set; }
    public string SubjectId { get; set; }

    public AcceptOfferRequest(string recordId, string subjectId)
    {
        RecordId = recordId;
        SubjectId = subjectId;
    }
}