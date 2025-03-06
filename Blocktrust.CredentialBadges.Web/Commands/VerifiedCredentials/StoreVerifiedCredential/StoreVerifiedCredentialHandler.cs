using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;

public class StoreVerifiedCredentialHandler : IRequestHandler<StoreVerifiedCredentialRequest, Result<VerifiedCredential>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<StoreVerifiedCredentialHandler> _logger;

    public StoreVerifiedCredentialHandler(ILogger<StoreVerifiedCredentialHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<VerifiedCredential>> Handle(StoreVerifiedCredentialRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Construct the domain model from the request, including Claims
        var verifiedCredential = new VerifiedCredential
        {
            Id = Guid.NewGuid(),
            Image = request.Image,
            Credential = request.Credential,
            Status = request.Status,
            ValidFrom = request.ValidFrom.ToUniversalTime(),
            ValidUntil = request.ValidUntil.ToUniversalTime(),
            Issuer = request.Issuer,
            Domain = request.Domain,
            Claims = request.Claims,
            SubjectId = request.SubjectId,
            SubjectName = request.SubjectName,
            TemplateId = string.IsNullOrEmpty(request.Image)?  "noimage_no_description_light": "image_description_light"
        };

        var entity = verifiedCredential.ToEntity();

        try
        {
            context.Set<VerifiedCredentialEntity>().Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            // Convert back to domain model (for returning to caller)
            var credential = VerifiedCredential.FromEntity(entity);
            return Result.Ok(credential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing credential");
            return Result.Fail<VerifiedCredential>("An error occurred while storing the credential");
        }
    }
}
