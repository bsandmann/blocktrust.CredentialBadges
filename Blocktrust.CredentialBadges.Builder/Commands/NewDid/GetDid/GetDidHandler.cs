// using MediatR;
// using FluentResults;
// using Blocktrust.CredentialBadges.IdentusClientApi;
//
// namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;
//
// public class GetDidHandler : IRequestHandler<GetDidRequest, Result<string>>
// {
//     private readonly IdentusClient _identusClient;
//
//     public GetDidHandler(IdentusClient identusClient)
//     {
//         _identusClient = identusClient;
//     }
//
//     public async Task<Result<string>> Handle(GetDidRequest request, CancellationToken cancellationToken)
//     {
//         try
//         {
//             // Call the IdentusClient to get the managed DIDs
//             var managedDids = await _identusClient.GetDidRegistrarDidsAsync(0, 1, cancellationToken);
//
//             // Ensure there's at least one DID returned
//             if (managedDids.Contents != null && managedDids.Contents.Any())
//             {
//                 // Return the first DID
//                 return Result.Ok(managedDids.Contents.First().Did);
//             }
//             else
//             {
//                 return Result.Fail<string>("No DIDs found in the wallet.");
//             }
//         }
//         catch (ApiException ex)
//         {
//             return Result.Fail<string>(ex.Message);
//         }
//     }
// }