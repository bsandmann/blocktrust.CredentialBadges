using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;

public class GetOfferByThIdRequest : IRequest<Result<IssueCredentialRecord>>
{
    public string ThId { get; set; }

    public GetOfferByThIdRequest(string thId)
    {
        ThId = thId;
    }
}