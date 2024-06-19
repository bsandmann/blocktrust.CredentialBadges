using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class OfferResponse
{
    public bool Success { get; set; }
    public string RecordId { get; set; }
    public string Thid { get; set; }
    
    public string Message { get; set; }
}

public class CreateOfferRequest : CreateIssueCredentialRecordRequest, IRequest<Result<OfferResponse>>
{
}