using Blocktrust.CredentialBadges.Builder.Commands.Offers.AcceptOffer;
using Blocktrust.CredentialBadges.Builder.Commands.Offers.GetOfferByThId;
using Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.UpdateBuilderCredential;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;
using Blocktrust.CredentialBadges.Builder.Enums;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.AcceptBuilderCredential;

public class AcceptBuilderCredentialHandler : IRequestHandler<AcceptBuilderCredentialRequest, Result<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<AcceptBuilderCredentialHandler> _logger;

    public AcceptBuilderCredentialHandler(ApplicationDbContext context, IMediator mediator, ILogger<AcceptBuilderCredentialHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<BuilderCredential>> Handle(AcceptBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        // Fetch the credential by ThId
        var offerResult = await _mediator.Send(new GetOfferByThIdRequest(request.ThId, request.ApiKey), cancellationToken);
        
        if (offerResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to fetch offer by ThId");
        }

        var issueCredentialRecord = offerResult.Value;

        // Accept the offer using RecordId and SubjectId
        var acceptResult = await _mediator.Send(new AcceptOfferRequest(issueCredentialRecord.RecordId, request.SubjectId, request.ApiKey), cancellationToken);

        if (acceptResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to accept offer");
        }

        // Retrieve the credential entity using ThId
        var entity = await _context.BuilderCredentials
            .FirstOrDefaultAsync(c => c.ThId.ToString() == request.ThId, cancellationToken);

        if (entity == null)
        {
            return Result.Fail<BuilderCredential>("Credential not found");
        }

        // Update the credential status to CredentialReceived and set RecordIdOnAgent
        var updateRequest = new UpdateBuilderCredentialRequest
        {
            CredentialId = entity.CredentialId,
            Status = EBuilderCredentialStatus.RequestSent,
            RecordIdOnAgent = Guid.Parse(issueCredentialRecord.RecordId)
        };

        var updateResult = await _mediator.Send(updateRequest, cancellationToken);

        if (updateResult.IsFailed)
        {
            return Result.Fail<BuilderCredential>("Failed to update credential");
        }

        return Result.Ok(updateResult.Value);
    }
}
