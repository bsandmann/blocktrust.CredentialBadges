namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.RegisterEntity;

using Blocktrust.CredentialBadges.Builder.Common;
using Blocktrust.CredentialBadges.IdentusClientApi;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

public class RegisterEntityHandler : IRequestHandler<RegisterEntityRequest, Result<Guid>>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<RegisterEntityHandler> _logger;

    public RegisterEntityHandler(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ILogger<RegisterEntityHandler> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("UserAgent");

        // Set the base URL and header for the HttpClient
        _httpClient.BaseAddress = new Uri(_appSettings.UserAgentBaseUrl);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-admin-api-key", _appSettings.UserAgentAdminKey);
    }

    public async Task<Result<Guid>> Handle(RegisterEntityRequest request, CancellationToken cancellationToken)
    {
        var identusClient = new IdentusClient(_httpClient)
        {
            BaseUrl = _appSettings.UserAgentBaseUrl // Ensure the base URL is set
        };

        try
        {
            var createEntityRequest = new CreateEntityRequest
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                WalletId = request.WalletId
            };

            var response = await identusClient.CreateEntityAsync(createEntityRequest, cancellationToken);
            return Result.Ok(response.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            return Result.Fail<Guid>(ex.Message);
        }
    }
}