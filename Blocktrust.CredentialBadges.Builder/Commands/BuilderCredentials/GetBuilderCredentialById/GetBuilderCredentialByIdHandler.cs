using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Domain;

namespace Blocktrust.CredentialBadges.Builder.Commands.BuilderCredentials.GetBuilderCredentialById;

public class GetBuilderCredentialByIdHandler : IRequestHandler<GetBuilderCredentialByIdRequest, Result<BuilderCredential>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetBuilderCredentialByIdHandler> _logger;

    public GetBuilderCredentialByIdHandler(ApplicationDbContext context, ILogger<GetBuilderCredentialByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<BuilderCredential>> Handle(GetBuilderCredentialByIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.BuilderCredentials
                .SingleOrDefaultAsync(bc => bc.CredentialId == request.CredentialId, cancellationToken);

            if (entity == null)
            {
                return Result.Fail<BuilderCredential>("Builder credential not found");
            }

            var credential = BuilderCredential.FromEntity(entity);

            return Result.Ok(credential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving builder credential by ID");
            return Result.Fail<BuilderCredential>("Failed to retrieve builder credential");
        }
    }
}