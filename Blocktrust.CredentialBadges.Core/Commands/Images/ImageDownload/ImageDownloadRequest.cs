namespace Blocktrust.CredentialBadges.Core.Commands.Images.ImageDownload;

public class ImageDownloadRequest
{
    public string ImageUrl { get; set; }

    public ImageDownloadRequest(string imageUrl)
    {
        ImageUrl = imageUrl;
    }
}