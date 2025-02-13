namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCache
{
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Blocktrust.CredentialBadges.Web.Entities;
    using Blocktrust.CredentialBadges.Web.Domain;
    using LazyCache;
    using System;

    public class UpdateCacheHandler : IRequestHandler<UpdateCacheRequest, Result>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IAppCache _cache;

        public UpdateCacheHandler(IServiceScopeFactory serviceScopeFactory, IAppCache cache)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cache = cache;
        }

        public async Task<Result> Handle(UpdateCacheRequest request, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Count the total number of credentials we have to process.
                var totalCount = await context.VerifiedCredentials.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    return Result.Ok(); // Nothing to update
                }

                var batchSize = request.BatchSize;
                var processed = 0;

                // Load credentials in batches
                while (processed < totalCount)
                {
                    var credentials = await context.VerifiedCredentials
                        .AsNoTracking()
                        .OrderBy(c => c.StoredCredentialId)
                        .Skip(processed)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);

                    foreach (var entity in credentials)
                    {
                        var credential = VerifiedCredential.FromEntity(entity);
                        var cacheKey = $"GetVerifiedCredentialById_{entity.StoredCredentialId}";

                        _cache.Add(cacheKey, credential, TimeSpan.FromMinutes(30));
                    }

                    processed += credentials.Count;
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Result.Fail(new Error("Failed to update cache").CausedBy(ex));
            }
        }
    }
}
