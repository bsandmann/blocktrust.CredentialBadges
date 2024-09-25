using Moq;
using Microsoft.Extensions.Logging;
using Emgu.CV;
using Emgu.CV.Structure;
using Blocktrust.CredentialBadges.Builder.Services;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace Blocktrust.CredentialBadges.Builder.Tests.Services;

public class ImageProcessingServiceTests
{
    private readonly Mock<ILogger<ImageProcessingService>> _loggerMock;
    private readonly ImageProcessingService _service;

    public ImageProcessingServiceTests()
    {
        _loggerMock = new Mock<ILogger<ImageProcessingService>>();
        _service = new ImageProcessingService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("image/png", 100, 100)]
    [InlineData("image/jpeg", 200, 150)]
    // [InlineData("image/gif", 150, 200)]
    public async Task ProcessImageAsync_ValidImage_ReturnsBase64String(string contentType, int targetWidth, int targetHeight)
    {
        // Arrange
        using var image = new Mat(400, 300, DepthType.Cv8U, 3);
        image.SetTo(new MCvScalar(255, 0, 0)); // Red color
        var ext = contentType.Split('/')[1];
        using var vectorOfByte = new VectorOfByte();
        CvInvoke.Imencode($".{ext}", image, vectorOfByte);
        using var ms = new MemoryStream(vectorOfByte.ToArray());

        // Act
        var result = await _service.ProcessImageAsync(ms, contentType, targetWidth, targetHeight);

        // Assert
        Assert.StartsWith($"data:{contentType};base64,", result);
        Assert.True(result.Length > 100); // Ensure we have a substantial base64 string
    }

    [Fact]
    public async Task ProcessImageAsync_EmptyStream_ThrowsArgumentException()
    {
        // Arrange
        using var emptyStream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessImageAsync(emptyStream, "image/png", 100, 100));
    }

    [Fact]
    public async Task ProcessImageAsync_UnsupportedContentType_ThrowsArgumentException()
    {
        // Arrange
        using var image = new Mat(100, 100, DepthType.Cv8U, 3);
        using var vectorOfByte = new VectorOfByte();
        CvInvoke.Imencode(".png", image, vectorOfByte);
        using var ms = new MemoryStream(vectorOfByte.ToArray());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessImageAsync(ms, "image/bmp", 100, 100));
    }

    [Fact]
    public async Task ProcessImageAsync_ResizesImage()
    {
        // Arrange
        using var image = new Mat(400, 200, DepthType.Cv8U, 3);
        using var vectorOfByte = new VectorOfByte();
        CvInvoke.Imencode(".png", image, vectorOfByte);
        using var ms = new MemoryStream(vectorOfByte.ToArray());

        // Act
        var result = await _service.ProcessImageAsync(ms, "image/png", 100, 100);

        // Assert
        Assert.StartsWith("data:image/png;base64,", result);
            
        // Decode the base64 string and check the dimensions
        var base64Data = result.Split(',')[1];
        var imageData = Convert.FromBase64String(base64Data);
        using var processedImage = new Mat();
        CvInvoke.Imdecode(imageData, ImreadModes.Unchanged, processedImage);
            
        Assert.Equal(100, processedImage.Width);
        Assert.Equal(100, processedImage.Height);
    }

    [Fact]
    public async Task ProcessImageAsync_HandlesTransparency()
    {
        // Arrange
        using var image = new Mat(100, 100, DepthType.Cv8U, 4); // 4 channels for RGBA
        image.SetTo(new MCvScalar(255, 0, 0, 128)); // Semi-transparent red
        using var vectorOfByte = new VectorOfByte();
        CvInvoke.Imencode(".png", image, vectorOfByte);
        using var ms = new MemoryStream(vectorOfByte.ToArray());

        // Act
        var result = await _service.ProcessImageAsync(ms, "image/png", 50, 50);

        // Assert
        Assert.StartsWith("data:image/png;base64,", result);
            
        // Decode the base64 string and check for transparency
        var base64Data = result.Split(',')[1];
        var imageData = Convert.FromBase64String(base64Data);
        using var processedImage = new Mat();
        CvInvoke.Imdecode(imageData, ImreadModes.Unchanged, processedImage);
            
        Assert.Equal(4, processedImage.NumberOfChannels); // Ensure we still have an alpha channel
    }
}