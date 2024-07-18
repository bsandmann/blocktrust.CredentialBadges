using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;

public class GetAllBuilderCredentialsHandler : IRequestHandler<GetAllBuilderCredentialsRequest, List<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetAllBuilderCredentialsHandler> _logger;

    public GetAllBuilderCredentialsHandler(ApplicationDbContext context, ILogger<GetAllBuilderCredentialsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BuilderCredential>> Handle(GetAllBuilderCredentialsRequest request, CancellationToken cancellationToken)
    {
        var entities = await _context.BuilderCredentials.ToListAsync(cancellationToken);
        var domainModels = entities.Select(entity => BuilderCredential.FromEntity(entity)).ToList();

        _logger.LogInformation("{Count} BuilderCredentials retrieved", domainModels.Count);

        return domainModels;
    }
}