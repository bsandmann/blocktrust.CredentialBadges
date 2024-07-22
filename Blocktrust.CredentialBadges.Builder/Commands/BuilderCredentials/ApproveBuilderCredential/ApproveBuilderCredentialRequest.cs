using Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
using Blocktrust.CredentialBadges.Builder.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.ApproveBuilderCredential;

public class ApproveBuilderCredentialRequest : CreateOfferRequest, IRequest<Result<BuilderCredential>>
{
    public Guid CredentialId { get; set; }
}