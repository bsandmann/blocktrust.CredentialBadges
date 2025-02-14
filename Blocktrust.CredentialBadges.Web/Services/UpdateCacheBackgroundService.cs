namespace Blocktrust.CredentialBadges.Web.Services
{
    using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateCache;
    using MediatR;

    public class UpdateCacheBackgroundService : BackgroundService
    {
        private readonly ILogger<UpdateCacheBackgroundService> _logger;
        private readonly IMediator _mediator;

        // Every 25 minutes
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(25);

        public UpdateCacheBackgroundService(
            ILogger<UpdateCacheBackgroundService> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting cache update for all credentials...");

                    var result = await _mediator.Send(new UpdateCacheRequest(), stoppingToken);

                    if (result.IsFailed)
                    {
                        _logger.LogError("Cache update failed: {Error}", result.ToString());
                    }
                    else
                    {
                        _logger.LogInformation("Successfully updated cache for all credentials at {Time}",
                            DateTimeOffset.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the credential cache.");
                }

                // Wait 25 minutes until the next cycle
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}