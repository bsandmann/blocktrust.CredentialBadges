using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;

namespace Blocktrust.CredentialBadges.Builder.Services;

public class ImageProcessingService
{
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessImageAsync(Stream imageStream, string contentType, int targetWidth, int targetHeight)
    {
        try
        {
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            ms.Position = 0;

            using var originalImage = new Mat();
            CvInvoke.Imdecode(ms.ToArray(), ImreadModes.Unchanged, originalImage);

            if (originalImage.IsEmpty)
            {
                throw new ArgumentException("Unable to decode the image.");
            }

            var aspectRatio = (double)originalImage.Width / originalImage.Height;
            var newWidth = targetWidth;
            var newHeight = targetHeight;

            if (aspectRatio > 1)
            {
                newHeight = (int)(targetHeight / aspectRatio);
            }
            else
            {
                newWidth = (int)(targetWidth * aspectRatio);
            }

            using var resizedImage = new Mat();
            CvInvoke.Resize(originalImage, resizedImage, new Size(newWidth, newHeight), 0, 0, Inter.Linear);

            using var paddedImage = new Mat(new Size(targetWidth, targetHeight), DepthType.Cv8U, originalImage.NumberOfChannels);
            paddedImage.SetTo(new MCvScalar(0, 0, 0, 0)); // Transparent background

            var roi = new Rectangle((targetWidth - newWidth) / 2, (targetHeight - newHeight) / 2, newWidth, newHeight);
            using var roiMat = new Mat(paddedImage, roi);
            resizedImage.CopyTo(roiMat);

            using var encodedImage = new VectorOfByte();
            CvInvoke.Imencode(GetFileExtension(contentType), paddedImage, encodedImage);

            var base64 = Convert.ToBase64String(encodedImage.ToArray());
            return $"data:{contentType};base64,{base64}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image with Emgu CV.");
            throw;
        }
    }

    private string GetFileExtension(string contentType)
    {
        return contentType switch
        {
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/gif" => ".gif",
            _ => throw new ArgumentException("Unsupported image format")
        };
    }
}