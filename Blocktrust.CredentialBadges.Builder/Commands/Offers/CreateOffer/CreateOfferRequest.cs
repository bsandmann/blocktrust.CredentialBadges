using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class OfferResponse
{
    public bool success { get; set; }
    public string recordId { get; set; }
    public string thid { get; set; }
    
    public string message { get; set; }
}
public class CreateOfferRequest : CreateIssueCredentialRecordRequest, IRequest<Result<OfferResponse>>
{
}