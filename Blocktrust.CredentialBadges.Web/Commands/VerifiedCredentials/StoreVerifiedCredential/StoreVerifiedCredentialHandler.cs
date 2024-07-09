using Blocktrust.CredentialBadges.Web.Entities;
using Blocktrust.CredentialBadges.Web.Domain;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.StoreVerifiedCredential;

public class StoreVerifiedCredentialHandler : IRequestHandler<StoreVerifiedCredentialRequest, Result<VerifiedCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StoreVerifiedCredentialHandler> _logger;

    public StoreVerifiedCredentialHandler(ApplicationDbContext context, ILogger<StoreVerifiedCredentialHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<VerifiedCredential>> Handle(StoreVerifiedCredentialRequest request, CancellationToken cancellationToken)
    {
        var entity = new VerifiedCredentialEntity
        {
            StoredCredentialId = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Image = request.Image,
            Credential = request.Credential,
            Status = request.Status,
            //convert to utc
            ValidFrom = request.ValidFrom.ToUniversalTime(),
            ValidUntil = request.ValidUntil.ToUniversalTime(),
            Issuer = request.Issuer
            
        };

        try
        {
            _context.Set<VerifiedCredentialEntity>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

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