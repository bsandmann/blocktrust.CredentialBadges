using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentAssertions;
using System.Text.Json.Nodes;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification.DidKeyVerification;

public class DIDKeyUtilsTests
{
    [Fact]
    public void CombineHashes_TwoHashes_ReturnsCombinedHash()
    {
        // Arrange
        byte[] hash1 = new byte[] { 1, 2, 3, 4 };
        byte[] hash2 = new byte[] { 5, 6, 7, 8 };
        byte[] expectedCombined = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        // Act
        var result = DIDKeyUtils.CombineHashes(hash1, hash2);

        // Assert
        result.Should().BeEquivalentTo(expectedCombined);
    }

    [Fact]
    public void ExtractPublicKeyMultibase_ValidDIDKey_ReturnsCorrectMultibase()
    {
        // Arrange
        string didKey = "did:key:z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";

        // Act
        var result = DIDKeyUtils.ExtractPublicKeyMultibase(didKey);

        // Assert
        result.Should().Be("z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv");
    }

    [Fact]
    public void ConvertMultibaseToPublicKey_ValidMultibase_ReturnsCorrectPublicKey()
    {
        // Arrange
        string multibase = "z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";

        // Act
        var result = DIDKeyUtils.ConvertMultibaseToPublicKey(multibase);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(32); // Ed25519 public key length
    }

    [Fact]
    public void RemoveProofFromCredential_ValidCredential_ReturnsCredentialWithoutProof()
    {
        // Arrange
        var credentialObject = new JsonObject
        {
            ["id"] = "http://example.edu/credentials/3732",
            ["proof"] = new JsonObject
            {
                ["type"] = "Ed25519Signature2018",
                ["created"] = "2021-06-18T21:19:10Z",
                ["proofPurpose"] = "assertionMethod",
                ["verificationMethod"] = "did:key:z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv#z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv",
                ["proofValue"] = "zQeVbY4oey5q7D3c6rJ9wVTSStb4xuD2inRcYFNL6bETPdHGLWAe3rFdNGDcqzgcsV5Y8GBo7j1yEaBeSkMw2YmW2"
            }
        };

        // Act
        var result = DIDKeyUtils.RemoveProofFromCredential(credentialObject);

        // Assert
        result.Should().NotContainKey("proof");
        result.Should().ContainKey("id");
    }

    [Fact]
    public void CanonicalizeCredential_ValidCredential_ReturnsCanonicalizedString()
    {
        // Arrange
        string credentialJson = TestDidKeyCredentials.ValidCredential;
        var credentialObject = JsonNode.Parse(credentialJson).AsObject();
        // Act
        var result = DIDKeyUtils.CanonicalizeCredential(credentialObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
        // Add more specific assertions based on the expected canonicalized output
    }
}