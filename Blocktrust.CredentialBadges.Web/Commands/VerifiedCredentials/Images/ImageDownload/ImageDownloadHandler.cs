using FluentResults;

namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.Images.ImageDownload;

public class ImageDownloadHandler
{
    private readonly HttpClient _httpClient;

    public ImageDownloadHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<byte[]>> HandleAsync(ImageDownloadRequest request)
    {
        try
        {
            var response = await _httpClient.GetAsync(request.ImageUrl);
            response.EnsureSuccessStatusCode();
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            return Result.Ok(imageBytes);
        }
        catch (HttpRequestException ex)
        {
            return Result.Fail(new Error("Failed to download image").CausedBy(ex));
        }
    }
}