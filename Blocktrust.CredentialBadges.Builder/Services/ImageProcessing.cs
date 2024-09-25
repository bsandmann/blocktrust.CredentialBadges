using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Blocktrust.CredentialBadges.Builder.Services;

public class ImageProcessingService
{
    public static async Task<string> ProcessImageAsync(Stream imageStream, string contentType, int targetWidth, int targetHeight)
    {
        using var image = await Image.LoadAsync(imageStream);
            
        // Resize and pad the image
        image.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.Pad
            })
        );

        // Determine the output format
        IImageEncoder encoder = contentType switch
        {
            "image/png" => new PngEncoder(),
            "image/jpeg" => new JpegEncoder(),
            "image/gif" => new GifEncoder(),
            _ => throw new ArgumentException("Unsupported image format")
        };

        using var ms = new MemoryStream();
        await image.SaveAsync(ms, encoder);
        return Convert.ToBase64String(ms.ToArray());
    }
}