namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageConversion
{
    public class ImageConversionRequest
    {
        public string ImageUrl { get; set; }

        public ImageConversionRequest(string imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}