using Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageDownload;
using FluentResults;

namespace Blocktrust.CredentialBadges.Web.Services.Images;

public class ConvertHostedImageToBase64
{
    private readonly ImageDownloadHandler _imageDownloadHandler;
    private readonly ImageBytesToBase64 _imageBytesToBase64;

    public ConvertHostedImageToBase64(ImageDownloadHandler imageDownloadHandler, ImageBytesToBase64 imageBytesToBase64)
    {
        _imageDownloadHandler = imageDownloadHandler;
        _imageBytesToBase64 = imageBytesToBase64;
    }

    public async Task<Result<string>> ConvertAsync(string imageUrl)
    {
        var request = new ImageDownloadRequest(imageUrl);
        var downloadResult = await _imageDownloadHandler.HandleAsync(request);

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