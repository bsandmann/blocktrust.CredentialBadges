namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageDownload;

public class ImageDownloadRequest
{
    public string ImageUrl { get; set; }

    public ImageDownloadRequest(string imageUrl)
    {
        ImageUrl = imageUrl;
    }
}