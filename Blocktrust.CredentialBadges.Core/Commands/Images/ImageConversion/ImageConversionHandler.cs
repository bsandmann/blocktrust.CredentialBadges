// using FluentResults;
// using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageDownload;
// using Blocktrust.CredentialBadges.Web.Services.Images;
// using MediatR;
//
// namespace Blocktrust.CredentialBadges.Web.Commands.Images.ImageConversion;
//
// public class ImageConversionHandler : IRequestHandler<ImageConversionRequest, Result<string>>
// {
//     private readonly ImageDownloadHandler _imageDownloadHandler;
//     private readonly ImageBytesToBase64 _imageBytesToBase64;
//
//     public ImageConversionHandler(ImageDownloadHandler imageDownloadHandler, ImageBytesToBase64 imageBytesToBase64)
//     {
//         _imageDownloadHandler = imageDownloadHandler;
//         _imageBytesToBase64 = imageBytesToBase64;
//     }
//
//     public async Task<Result<string>> Handle(ImageConversionRequest request, CancellationToken cancellationToken)
//     {
//         var downloadRequest = new ImageDownloadRequest(request.ImageUrl);
//         var downloadResult = await _imageDownloadHandler.HandleAsync(downloadRequest);
//
//         if (downloadResult.IsFailed)
//         {
//             return Result.Fail(downloadResult.Errors);
//         }
//
//         var conversionResult = _imageBytesToBase64.Convert(downloadResult.Value);
//         if (conversionResult.IsFailed)
//         {
//             return Result.Fail(conversionResult.Errors);
//         }
//
//         return Result.Ok(conversionResult.Value);
//     }
// }