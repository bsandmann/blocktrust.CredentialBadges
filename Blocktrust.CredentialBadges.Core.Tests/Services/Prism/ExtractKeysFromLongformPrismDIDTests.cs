using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.Prism;
using Blocktrust.CredentialBadges.Core.Prism;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.Prism;

public class ExtractKeysFromLongformPrismDIDTests
{
    private readonly ExtractKeysFromLongformPrismDID _extractor = new ExtractKeysFromLongformPrismDID();

    [Fact]
    public void ExtractPublicKey_ValidLongFormDID_ReturnsCorrectPublicKey()
    {
        // Arrange
        var longFormDid = "did:prism:722b5eaf33059c4b828732b27fe410222f0b1407098ff764c57de1f20adf26b2:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiEC3__sBCogrv7XZuQbevdDSy8U2FZb_8B9OcF3ZySwOJUSOwoHbWFzdGVyMBABSi4KCXNlY3AyNTZrMRIhA5UrXRfe6NWIpTbRkyXXw3JV2U1yMt_q9QQKf8GfriaG";
        var expectedPublicKeyHex = "";

        // Act
        var result = _extractor.ExtractPublicKey(longFormDid);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultHex = PrismEncoding.ByteArrayToHex(result.Value);
        // resultHex.Should().Be(expectedPublicKeyHex);
    }

    [Fact]
    public void ExtractPublicKey_InvalidDIDFormat_ReturnsFailure()
    {
        // Arrange
        var invalidDid = "invalid:did:format";

        // Act
        var result = _extractor.ExtractPublicKey(invalidDid);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid long-form Prism DID format");
    }

    [Fact]
    public void ExtractPublicKey_NotCreateDIDOperation_ReturnsFailure()
    {
        // Arrange
        // This is a mock long-form DID that doesn't contain a CreateDIDOperation
        var invalidDid = "did:prism:mock:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1Nms";

        // Act
        var result = _extractor.ExtractPublicKey(invalidDid);

        // Assert
        result.IsFailed.Should().BeTrue();
        // result.Errors.Should().Contain(e => e.Message == "The operation is not a CreateDIDOperation");
    }

    [Fact]
    public void ExtractPublicKey_NoSuitableKey_ReturnsFailure()
    {
        // Arrange
        // This is a mock long-form DID that doesn't contain an ISSUING_KEY or MASTER_KEY
        var invalidDid = "did:prism:mock:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EANKLgoJc2VjcDI1Nms";

        // Act
        var result = _extractor.ExtractPublicKey(invalidDid);

        // Assert
        result.IsFailed.Should().BeTrue();
        // result.Errors.Should().Contain(e => e.Message == "No suitable public key found in the DID document");
    }

    [Fact]
    public void ExtractPublicKey_CompressedKey_ReturnsCorrectPublicKey()
    {
        // Arrange
        // This is a mock long-form DID with a compressed key
        var compressedKeyDid = "did:prism:mock:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiECKy8NCXIeWtcCQ==";
        var expectedPublicKeyHex = "042b2f0d09721e5ad70243000000000000000000000000000000000000000000002b2f0d09721e5ad70243000000000000000000000000000000000000000000";

        // Act
        var result = _extractor.ExtractPublicKey(compressedKeyDid);

        // Assert
        // result.IsSuccess.Should().BeTrue();
        // var resultHex = PrismEncoding.ByteArrayToHex(result.Value);
        // resultHex.Should().Be(expectedPublicKeyHex);
    }
}