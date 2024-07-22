using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.UpdateBuilderCredential;

public class UpdateBuilderCredentialHandler : IRequestHandler<UpdateBuilderCredentialRequest, Result<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateBuilderCredentialHandler> _logger;

    public UpdateBuilderCredentialHandler(ApplicationDbContext context, ILogger<UpdateBuilderCredentialHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<BuilderCredential>> Handle(UpdateBuilderCredentialRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.BuilderCredentials
            .FirstOrDefaultAsync(c => c.CredentialId == request.CredentialId, cancellationToken);

        if (entity == null)
        {
            return Result.Fail<BuilderCredential>("Credential not found");
        }

        // Update only the fields that are provided in the request
        if (request.Date.HasValue)
        {
            entity.Date = request.Date.Value;
        }
        if (request.Label != null)
        {
            entity.Label = request.Label;
        }
        if (request.SubjectDid != null)
        {
            entity.SubjectDid = request.SubjectDid;
        }
        if (request.IssuerDid != null)
        {
            entity.IssuerDid = request.IssuerDid;
        }
        if (request.Status.HasValue)
        {
            entity.Status = request.Status.Value;
        }
        if (request.IssuerConnectionId.HasValue)
        {
            entity.IssuerConnectionId = request.IssuerConnectionId.Value;
        }
        if (request.SubjectConnectionId.HasValue)
        {
            entity.SubjectConnectionId = request.SubjectConnectionId.Value;
        }
        if (request.CredentialSubject != null)
        {
            entity.CredentialSubject = request.CredentialSubject;
        }
        if (request.UserId != null)
        {
            entity.UserId = request.UserId;
        }
        if (request.ThId.HasValue)
        {
            entity.ThId = request.ThId.Value;
        }
        if (request.RecordIdOnAgent.HasValue)
        {
            entity.RecordIdOnAgent = request.RecordIdOnAgent.Value;
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            var updatedCredential = BuilderCredential.FromEntity(entity);
            return Result.Ok(updatedCredential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating builder credential");
            return Result.Fail<BuilderCredential>("An error occurred while updating the builder credential");
        }
    }
}
