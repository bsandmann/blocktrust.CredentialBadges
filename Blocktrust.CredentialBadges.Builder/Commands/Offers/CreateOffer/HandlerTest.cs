//
//  using Blocktrust.CredentialBadges.IdentusClientApi;
// using FluentResults;
// using MediatR;
//
// namespace Blocktrust.CredentialBadges.Builder.Commands.Offers.CreateOffer;
//
// public class CreateOfferHandler : IRequestHandler<CreateOfferRequest, Result<string>>
// {
//     private readonly IdentusClient _identusClient;
//     private readonly ILogger<CreateOfferHandler> _logger;
//
//     public CreateOfferHandler(IdentusClient identusClient, ILogger<CreateOfferHandler> logger)
//     {
//         _identusClient = identusClient;
//         _logger = logger;
//     }
//
//     public async Task<Result<string>> Handle(CreateOfferRequest request, CancellationToken cancellationToken)
//     {
//         try
//         {
//
//             var response = await _identusClient.CreateCredentialOfferAsync(request.Claims, request.CredentialFormat, request.IssuingDID, request.ConnectionId, cancellationToken);
//
//             return Result.Ok(response);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error creating offer");
//             return Result.Fail(ex.Message);
//         }
//     }
// }
//
