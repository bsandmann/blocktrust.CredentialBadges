using FluentResults;
using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageDownload;
using Blocktrust.CredentialBadges.Web.Services.Images;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageConversion;

public class ImageConversionHandler
{
    private readonly ImageDownloadHandler _imageDownloadHandler;
    private readonly ImageBytesToBase64 _imageBytesToBase64;

    public ImageConversionHandler(ImageDownloadHandler imageDownloadHandler, ImageBytesToBase64 imageBytesToBase64)
    {
        _imageDownloadHandler = imageDownloadHandler;
        _imageBytesToBase64 = imageBytesToBase64;
    }

    public async Task<Result<string>> HandleAsync(ImageConversionRequest request)
    {
        var downloadRequest = new ImageDownloadRequest(request.ImageUrl);
        var downloadResult = await _imageDownloadHandler.HandleAsync(downloadRequest);

        if (downloadResult.IsFailed)
        {
            return Result.Fail(downloadResult.Errors);
        }

        var conversionResult = _imageBytesToBase64.Convert(downloadResult.Value);
        if (conversionResult.IsFailed)
        {
            return Result.Fail(conversionResult.Errors);
        }

        return Result.Ok(conversionResult.Value);
    }
}