using System.Text.Json;
using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;
using Xunit.Abstractions;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class DIDKeyResolverTests
{
    private readonly ITestOutputHelper _output;

    public DIDKeyResolverTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6", "FN5URrQRnKSaEjQvC8e6vpNGxivoocgPLgmzv_5JH2k")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB", "wFSE8ywxP0ROttCyGoNrYg7IwTG7101wOlABbzo9xxI")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH", "lJZrfAjkBXdfjebMHEUI9usidAPhAlssitLXR3OYxbI")]
    public void ResolveDidKey_ShouldReturnValidDidDocument_ForSpecificEd25519Keys(string didKey, string expectedXValue)
    {
        // Arrange
        var resolver = new DIDKeyResolver();

        // Act
        var result = resolver.ResolveDidKey(didKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();

        _output.WriteLine($"Generated DID Document:\n{result.Value}");

        var didDocument = JsonDocument.Parse(result.Value);
        var root = didDocument.RootElement;

        // Check DID Document structure
        root.TryGetProperty("context", out var contextElement).Should().BeTrue("context should be present");
        contextElement.EnumerateArray().Should().Contain(e => e.GetString() == "https://www.w3.org/ns/did/v1");
        contextElement.EnumerateArray().Should().Contain(e => e.GetString() == "https://w3id.org/security/suites/jws-2020/v1");

        root.TryGetProperty("id", out var idElement).Should().BeTrue("id should be present");
        idElement.GetString().Should().Be(didKey);

        root.TryGetProperty("verificationMethod", out var verificationMethodElement).Should().BeTrue("verificationMethod should be present");
        verificationMethodElement.GetArrayLength().Should().Be(1);

        // Check verification method
        var verificationMethod = verificationMethodElement[0];
        verificationMethod.TryGetProperty("id", out var vmIdElement).Should().BeTrue("verificationMethod.id should be present");
        vmIdElement.GetString().Should().Be($"{didKey}#{didKey.Substring(8)}");

        verificationMethod.TryGetProperty("type", out var vmTypeElement).Should().BeTrue("verificationMethod.type should be present");
        vmTypeElement.GetString().Should().Be("JsonWebKey2020");

        verificationMethod.TryGetProperty("controller", out var vmControllerElement).Should().BeTrue("verificationMethod.controller should be present");
        vmControllerElement.GetString().Should().Be(didKey);

        verificationMethod.TryGetProperty("publicKeyJwk", out var publicKeyJwkElement).Should().BeTrue("verificationMethod.publicKeyJwk should be present");
            
        publicKeyJwkElement.TryGetProperty("kty", out var ktyElement).Should().BeTrue("publicKeyJwk.kty should be present");
        ktyElement.GetString().Should().Be("OKP");

        publicKeyJwkElement.TryGetProperty("crv", out var crvElement).Should().BeTrue("publicKeyJwk.crv should be present");
        crvElement.GetString().Should().Be("Ed25519");

        publicKeyJwkElement.TryGetProperty("x", out var xElement).Should().BeTrue("publicKeyJwk.x should be present");
        xElement.GetString().Should().Be(expectedXValue);

        // Check verification relationships
        CheckVerificationRelationship(root, "assertionMethod", didKey);
        CheckVerificationRelationship(root, "authentication", didKey);
        CheckVerificationRelationship(root, "capabilityInvocation", didKey);
        CheckVerificationRelationship(root, "capabilityDelegation", didKey);

        // Ensure keyAgreement is not present
        root.TryGetProperty("keyAgreement", out var _).Should().BeFalse();
    }

    private void CheckVerificationRelationship(JsonElement root, string relationshipName, string didKey)
    {
        root.TryGetProperty(relationshipName, out var relationshipElement).Should().BeTrue($"{relationshipName} should be present");
        relationshipElement.GetArrayLength().Should().Be(1);
        relationshipElement[0].GetString().Should().Be($"{didKey}#{didKey.Substring(8)}");
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForInvalidDidKeyFormat()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var invalidDidKey = "invalid:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6";

        // Act
        var result = resolver.ResolveDidKey(invalidDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Invalid DID key format.");
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForMissingPublicKeyPart()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var invalidDidKey = "did:key:";

        // Act
        var result = resolver.ResolveDidKey(invalidDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "DID key is missing the public key part.");
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForUnsupportedKeyType()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var unsupportedDidKey = "did:key:z8iaMxQaDHjPHZtT4XWZemGXpMGv6PV3Xq2nXx57xBmWR"; // This is not a valid key type

        // Act
        var result = resolver.ResolveDidKey(unsupportedDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.StartsWith("Unsupported key type:"));
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForInvalidBase58Encoding()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var invalidEncodedDidKey = "did:key:z6MkInvalidBase58EncodingHere!!!";

        // Act
        var result = resolver.ResolveDidKey(invalidEncodedDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.StartsWith("Failed to decode base58 key:"));
    }
}