using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;

public class GetOfferByThIdRequest : IRequest<Result<IssueCredentialRecord>>
{
    public string ThId { get; set; }
    
    public string ApiKey { get; set; }

    public GetOfferByThIdRequest(string thId)
    {
        ThId = thId;
    }
    
    public GetOfferByThIdRequest(string thId, string apiKey)
    {
        ThId = thId;
        ApiKey = apiKey;
    }
    
}