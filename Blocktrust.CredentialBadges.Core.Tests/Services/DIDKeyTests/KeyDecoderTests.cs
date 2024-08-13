using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class KeyDecoderTests
{
    private readonly KeyDecoder _keyDecoder;

    public KeyDecoderTests()
    {
        _keyDecoder = new KeyDecoder();
    }

    [Theory]
    [InlineData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH")]
    public void DecodeKey_ShouldSucceed_ForValidDidKeys(string didKey)
    {
        // Act
        var result = _keyDecoder.DecodeKey(didKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(32);
    }

    [Fact]
    public void DecodeKey_ShouldFail_ForInvalidBase58Encoding()
    {
        // Arrange
        var invalidEncodedDidKey = "did:key:z6MkInvalidBase58EncodingHere!!!";

        // Act
        var result = _keyDecoder.DecodeKey(invalidEncodedDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.StartsWith("Failed to decode base58 key:"));
    }
}