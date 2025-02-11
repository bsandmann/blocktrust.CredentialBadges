using Blocktrust.CredentialBadges.Builder.Data.Entities;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;

using Data;
using Domain;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetBuilderCredentialsByUserIdHandler : IRequestHandler<GetBuilderCredentialsByUserIdRequest, Result<List<BuilderCredential>>>
{
    private IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<GetBuilderCredentialsByUserIdHandler> _logger;

    public GetBuilderCredentialsByUserIdHandler(ILogger<GetBuilderCredentialsByUserIdHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result<List<BuilderCredential>>> Handle(GetBuilderCredentialsByUserIdRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            IQueryable<BuilderCredentialEntity> query = context.BuilderCredentials
                .Where(c => c.UserId == request.UserId);

            if (!string.IsNullOrEmpty(request.SubjectDid))
            {
                query = query.Where(c => c.SubjectDid == request.SubjectDid);
            }

            var entities = await query.ToListAsync(cancellationToken);

            var credentials = entities
                .Select(entity => BuilderCredential.FromEntity(entity))
                .ToList();

            return Result.Ok(credentials);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving builder credentials for user ID: {UserId}, Subject DID: {SubjectDid}", request.UserId, request.SubjectDid);
            return Result.Fail<List<BuilderCredential>>("Failed to retrieve builder credentials for the specified user ID and Subject DID");
        }
    }
}