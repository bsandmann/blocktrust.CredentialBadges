// using MediatR;
// using FluentResults;
//
// namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;
//
// public class NewDidAndApiKeyHandler : IRequestHandler<NewDidAndApiKeyRequest, Result>
// {
//     private readonly IMediator _mediator;
//
//     public NewDidAndApiKeyHandler(IMediator mediator)
//     {
//         _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
//     }
//
//     public async Task<Result> Handle(NewDidAndApiKeyRequest request, CancellationToken cancellationToken)
//     {
//         try
//         {
//             // Step 1: Register a new wallet
//             var registerWalletRequest = new RegisterWalletRequest
//             {
//                 Seed = request.Seed,
//                 Name = request.WalletName
//             };
//             var walletIdResult = await _mediator.Send(registerWalletRequest, cancellationToken);
//             if (!walletIdResult.IsSuccess)
//             {
//                 return Result.Fail("Failed to register wallet.");
//             }
//             var walletId = walletIdResult.Value;
//
//             // Step 2: Register an entity (assuming using the walletId)
//             var registerEntityRequest = new RegisterEntityRequest
//             {
//                 Name = "EntityName", // Replace with actual entity name if needed
//                 WalletId = walletId
//             };
//             var entityIdResult = await _mediator.Send(registerEntityRequest, cancellationToken);
//             if (!entityIdResult.IsSuccess)
//             {
//                 return Result.Fail("Failed to register entity.");
//             }
//             var entityId = entityIdResult.Value;
//
//             // Step 3: Register an API key for the entity
//             var registerApiKeyRequest = new RegisterApiKeyRequest
//             {
//                 EntityId = entityId,
//                 ApiKey = GenerateApiKey() // Generate API Key
//             };
//             var apiKeyResult = await _mediator.Send(registerApiKeyRequest, cancellationToken);
//             if (!apiKeyResult.IsSuccess)
//             {
//                 return Result.Fail("Failed to register API key.");
//             }
//
//             // Step 4: Get DID using the API key
//             var getDidRequest = new GetDidRequest
//             {
//                 ApiKey = apiKeyResult.Value
//             };
//             var didResult = await _mediator.Send(getDidRequest, cancellationToken);
//             if (!didResult.IsSuccess)
//             {
//                 return Result.Fail("Failed to get DID.");
//             }
//             var did = didResult.Value;
//
//             // Return success result with DID and API Key
//             return Result.Ok();
//         }
//         catch (Exception ex)
//         {
//             // Handle exceptions if needed
//             return Result.Fail(ex.Message);
//         }
//     }
//
//     private string GenerateApiKey()
//     {
//         // Replace with actual API Key generation logic
//         return Guid.NewGuid().ToString("N"); // Example: Generate a new GUID as API Key
//     }
// }