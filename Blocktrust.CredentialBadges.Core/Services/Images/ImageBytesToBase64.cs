namespace Blocktrust.CredentialBadges.Core.Services.Images;

using FluentResults;

public class ImageBytesToBase64
{
    public Result<string> Convert(byte[] imageBytes)
    {
        try
        {
            var base64String = System.Convert.ToBase64String(imageBytes);
            return Result.Ok(base64String);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to convert image bytes to base64").CausedBy(ex));
        }
    }
}