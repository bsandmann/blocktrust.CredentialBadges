namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCache
{
    using MediatR;
    using FluentResults;

    /// <summary>
    /// Command for refreshing the cache with all VerifiedCredentials from the database.
    /// </summary>
    public class UpdateCacheRequest : IRequest<Result>
    {
        public int BatchSize { get; }

        /// <summary>
        /// Optional: specify the batch size. Defaults to 500 if not set.
        /// </summary>
        public UpdateCacheRequest(int batchSize = 500)
        {
            BatchSize = batchSize;
        }
    }
}