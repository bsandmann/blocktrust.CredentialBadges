using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetAllBuilderCredentials;

public class GetAllBuilderCredentialsHandler : IRequestHandler<GetAllBuilderCredentialsRequest, Result<List<BuilderCredential>>>
{
    private IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<GetAllBuilderCredentialsHandler> _logger;

    public GetAllBuilderCredentialsHandler(ILogger<GetAllBuilderCredentialsHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<List<BuilderCredential>>> Handle(GetAllBuilderCredentialsRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var entities = await context.BuilderCredentials
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