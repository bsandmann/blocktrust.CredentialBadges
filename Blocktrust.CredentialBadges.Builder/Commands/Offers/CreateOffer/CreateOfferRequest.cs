using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.IdentusClientApi;

namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;

public class CreateOfferRequest : CreateIssueCredentialRecordRequest, IRequest<Result<string>>
{
}