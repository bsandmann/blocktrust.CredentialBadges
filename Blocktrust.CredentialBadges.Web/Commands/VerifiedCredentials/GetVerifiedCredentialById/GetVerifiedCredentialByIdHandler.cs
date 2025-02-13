namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.GetVerifiedCredentialById;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Blocktrust.CredentialBadges.Web.Entities;
using LazyCache;  // import LazyCache
using System;

public class GetVerifiedCredentialByIdHandler : IRequestHandler<GetVerifiedCredentialByIdRequest, Result<VerifiedCredential>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppCache _cache;

    public GetVerifiedCredentialByIdHandler(IServiceScopeFactory serviceScopeFactory, IAppCache cache)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _cache = cache;
    }

    public async Task<Result<VerifiedCredential>> Handle(GetVerifiedCredentialByIdRequest request, CancellationToken cancellationToken)
    {
        // If the caller wants to skip the cache, do a direct DB lookup
        if (request.SkipCache)
        {
            return await GetCredentialDirectly(request.CredentialId, cancellationToken);
        }

        var cacheKey = $"GetVerifiedCredentialById_{request.CredentialId}";

        try
        {
            var resultFromCache = await _cache.GetOrAddAsync(
                cacheKey,
                async entry =>
                {
                    // Set up the desired cache expiration to 30 minutes
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                    // Actually fetch from DB inside the cache factory method
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var credentialEntity = await context.VerifiedCredentials.FindAsync(new object[] { request.CredentialId }, cancellationToken);

                    if (credentialEntity is null)
                    {
                        throw new InvalidOperationException($"Credential with ID {request.CredentialId} not found.");
                    }

                    return VerifiedCredential.FromEntity(credentialEntity);
                }
            );

            // Wrap cached result in FluentResults
            return Result.Ok(resultFromCache);
        }
        catch (InvalidOperationException ex)
        {
            // If the credential wasn't found, we return an error
            return Result.Fail(new Error(ex.Message));
        }
    }

    private async Task<Result<VerifiedCredential>> GetCredentialDirectly(Guid credentialId, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var credentialEntity = await context.VerifiedCredentials.FindAsync(new object[] { credentialId }, cancellationToken);
        if (credentialEntity is null)
        {
            return Result.Fail($"Credential with ID {credentialId} not found.");
        }

        return Result.Ok(VerifiedCredential.FromEntity(credentialEntity));
    }
}
