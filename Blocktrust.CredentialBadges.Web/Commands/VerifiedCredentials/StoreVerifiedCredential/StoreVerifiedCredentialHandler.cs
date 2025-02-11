using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;

public class StoreVerifiedCredentialHandler : IRequestHandler<StoreVerifiedCredentialRequest, Result<VerifiedCredential>>
{
    private IServiceScopeFactory _serviceScopeFactory;
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

        var entity = new VerifiedCredentialEntity
        {
            StoredCredentialId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Image = request.Image,
            Credential = request.Credential,
            Status = request.Status,
            ValidFrom = request.ValidFrom.ToUniversalTime(),
            ValidUntil = request.ValidUntil.ToUniversalTime(),
            Issuer = request.Issuer,
            Domain = request.Domain
        };

        try
        {
            context.Set<VerifiedCredentialEntity>().Add(entity);
            await context.SaveChangesAsync(cancellationToken);

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