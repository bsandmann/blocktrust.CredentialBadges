using Blocktrust.CredentialBadges.Web.Entities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateTemplateId;

public class UpdateCredentialTemplateIdHandler : IRequestHandler<UpdateCredentialTemplateIdRequest, Result>
{
    private IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UpdateCredentialTemplateIdHandler> _logger;

    public UpdateCredentialTemplateIdHandler(ILogger<UpdateCredentialTemplateIdHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result> Handle(UpdateCredentialTemplateIdRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var entity = await context.Set<VerifiedCredentialEntity>()
                .FirstOrDefaultAsync(vc => vc.StoredCredentialId == request.CredentialId, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("Credential with ID {CredentialId} not found", request.CredentialId);
                return Result.Fail($"Credential with ID {request.CredentialId} not found");
            }

            entity.TemplateId = request.TemplateId;

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated template ID for credential {CredentialId} to {TemplateId}", request.CredentialId, request.TemplateId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template ID for credential {CredentialId}", request.CredentialId);
            return Result.Fail("An error occurred while updating the credential's template ID");
        }
    }
}