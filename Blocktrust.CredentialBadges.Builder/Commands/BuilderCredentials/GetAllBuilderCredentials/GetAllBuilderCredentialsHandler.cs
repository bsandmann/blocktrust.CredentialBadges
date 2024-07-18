using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;

public class GetAllBuilderCredentialsHandler : IRequestHandler<GetAllBuilderCredentialsRequest, Result<List<BuilderCredential>>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetAllBuilderCredentialsHandler> _logger;

    public GetAllBuilderCredentialsHandler(ApplicationDbContext context, ILogger<GetAllBuilderCredentialsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<BuilderCredential>>> Handle(GetAllBuilderCredentialsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _context.BuilderCredentials
                .ToListAsync(cancellationToken);

            var credentials = entities
                .Select(entity => BuilderCredential.FromEntity(entity))
                .ToList();

            return Result.Ok(credentials);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all builder credentials");
            return Result.Fail<List<BuilderCredential>>("Failed to retrieve builder credentials");
        }
    }
}