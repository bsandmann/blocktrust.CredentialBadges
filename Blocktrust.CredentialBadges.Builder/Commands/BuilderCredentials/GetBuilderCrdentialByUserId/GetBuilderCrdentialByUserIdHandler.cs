using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialsByUserId;

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
            var entities = await _context.BuilderCredentials
                .Where(c => c.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            var credentials = entities
                .Select(entity => BuilderCredential.FromEntity(entity))
                .ToList();

            return Result.Ok(credentials);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving builder credentials for user ID: {UserId}", request.UserId);
            return Result.Fail<List<BuilderCredential>>("Failed to retrieve builder credentials for the specified user ID");
        }
    }
}