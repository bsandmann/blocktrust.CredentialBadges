using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.UpdateBuilderCredential;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.ApproveBuilderCredential;

public class ApproveBuilderCredentialHandler : IRequestHandler<ApproveBuilderCredentialRequest, Result<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<ApproveBuilderCredentialHandler> _logger;

    public ApproveBuilderCredentialHandler(ApplicationDbContext context, IMediator mediator, ILogger<ApproveBuilderCredentialHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<BuilderCredential>> Handle(ApproveBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        // Retrieve the credential entity
        var entity = await _context.BuilderCredentials
            .FirstOrDefaultAsync(c => c.CredentialId == request.CredentialId, cancellationToken);

        if (entity == null)
        {
            return Result.Fail<BuilderCredential>("Credential not found");
        }

        // Create offer
        var offerResult = await _mediator.Send(new CreateOfferRequest
        {
            // Pass necessary fields from the request
            // Assuming necessary fields are included in the ApproveBuilderCredentialRequest via inheritance
            // Populate fields as necessary, e.g., request.FieldName
        }, cancellationToken);

        if (offerResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to create offer");
        }

        var offerResponse = offerResult.Value;

        // Update credential status to WaitingAcceptance and set ThId
        var updateRequest = new UpdateBuilderCredentialRequest
        {
            CredentialId = request.CredentialId,
            Status = EBuilderCredentialStatus.WaitingAcceptance,
            ThId = Guid.Parse(offerResponse.Thid)
        };

        var updateResult = await _mediator.Send(updateRequest, cancellationToken);

        if (updateResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to update credential");
        }

        return Result.Ok(updateResult.Value);
    }
}
