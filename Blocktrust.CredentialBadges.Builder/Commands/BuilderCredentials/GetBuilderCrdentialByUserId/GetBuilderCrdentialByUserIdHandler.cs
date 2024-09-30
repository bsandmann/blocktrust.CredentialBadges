using Blocktrust.CredentialBadges.Builder.Data.Entities;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCrdentialByUserId;

using Data;
using Domain;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetBuilderCredentialsByUserIdHandler : IRequestHandler<GetBuilderCredentialsByUserIdRequest, Result<List<BuilderCredential>>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialsByUserIdHandler> _logger;

    public GetBuilderCredentialsByUserIdHandler(ApplicationDbContext context, ILogger<GetBuilderCredentialsByUserIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<BuilderCredential>>> Handle(GetBuilderCredentialsByUserIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<BuilderCredentialEntity> query = _context.BuilderCredentials
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