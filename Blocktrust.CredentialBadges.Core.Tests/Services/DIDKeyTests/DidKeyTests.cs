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

    [Fact]
    public void ResolveDidKey_ShouldReturnValidDidDocument_ForEd25519Key()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var didKey = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act
        var result = resolver.ResolveDidKey(didKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();

        _output.WriteLine($"Generated DID Document:\n{result.Value}");

        var didDocument = JsonDocument.Parse(result.Value);
        var root = didDocument.RootElement;

        // Check DID Document structure
        CheckContext(root);
        CheckId(root, didKey);
        CheckVerificationMethod(root, didKey);
        CheckVerificationRelationships(root, didKey);
        CheckKeyAgreement(root, didKey);
    }

    private void CheckContext(JsonElement root)
    {
        root.TryGetProperty("@context", out var contextElement).Should().BeTrue("@context should be present");
        contextElement.EnumerateArray().Should().Contain(e => e.GetString() == "https://www.w3.org/ns/did/v1");
        contextElement.EnumerateArray().Should().Contain(e => e.GetString() == "https://w3id.org/security/suites/ed25519-2020/v1");
        contextElement.EnumerateArray().Should().Contain(e => e.GetString() == "https://w3id.org/security/suites/x25519-2020/v1");
    }

    private void CheckId(JsonElement root, string didKey)
    {
        root.TryGetProperty("id", out var idElement).Should().BeTrue("id should be present");
        idElement.GetString().Should().Be(didKey);
    }

    private void CheckVerificationMethod(JsonElement root, string didKey)
    {
        root.TryGetProperty("verificationMethod", out var verificationMethodElement).Should().BeTrue("verificationMethod should be present");
        verificationMethodElement.GetArrayLength().Should().Be(1);

        var verificationMethod = verificationMethodElement[0];
        verificationMethod.TryGetProperty("id", out var vmIdElement).Should().BeTrue("verificationMethod.id should be present");
        vmIdElement.GetString().Should().Be($"{didKey}#{didKey.Substring(8)}");

        verificationMethod.TryGetProperty("type", out var vmTypeElement).Should().BeTrue("verificationMethod.type should be present");
        vmTypeElement.GetString().Should().Be("Ed25519VerificationKey2020");

        verificationMethod.TryGetProperty("controller", out var vmControllerElement).Should().BeTrue("verificationMethod.controller should be present");
        vmControllerElement.GetString().Should().Be(didKey);

        verificationMethod.TryGetProperty("publicKeyMultibase", out var publicKeyMultibaseElement).Should().BeTrue("verificationMethod.publicKeyMultibase should be present");
        publicKeyMultibaseElement.GetString().Should().Be(didKey.Substring(8));
    }

    private void CheckVerificationRelationships(JsonElement root, string didKey)
    {
        CheckVerificationRelationship(root, "authentication", didKey);
        CheckVerificationRelationship(root, "assertionMethod", didKey);
        CheckVerificationRelationship(root, "capabilityInvocation", didKey);
        CheckVerificationRelationship(root, "capabilityDelegation", didKey);
    }

    private void CheckVerificationRelationship(JsonElement root, string relationshipName, string didKey)
    {
        root.TryGetProperty(relationshipName, out var relationshipElement).Should().BeTrue($"{relationshipName} should be present");
        relationshipElement.GetArrayLength().Should().Be(1);
        relationshipElement[0].GetString().Should().Be($"{didKey}#{didKey.Substring(8)}");
    }

    private void CheckKeyAgreement(JsonElement root, string didKey)
    {
        root.TryGetProperty("keyAgreement", out var keyAgreementElement).Should().BeTrue("keyAgreement should be present");
        keyAgreementElement.GetArrayLength().Should().Be(1);

        var keyAgreement = keyAgreementElement[0];
        keyAgreement.TryGetProperty("id", out var kaIdElement).Should().BeTrue("keyAgreement.id should be present");
        kaIdElement.GetString().Should().StartWith(didKey);

        keyAgreement.TryGetProperty("type", out var kaTypeElement).Should().BeTrue("keyAgreement.type should be present");
        kaTypeElement.GetString().Should().Be("X25519KeyAgreementKey2020");

        keyAgreement.TryGetProperty("controller", out var kaControllerElement).Should().BeTrue("keyAgreement.controller should be present");
        kaControllerElement.GetString().Should().Be(didKey);

        keyAgreement.TryGetProperty("publicKeyMultibase", out var kaPublicKeyMultibaseElement).Should().BeTrue("keyAgreement.publicKeyMultibase should be present");
        kaPublicKeyMultibaseElement.GetString().Should().StartWith("z");
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForInvalidDidKeyFormat()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var invalidDidKey = "invalid:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act
        var result = resolver.ResolveDidKey(invalidDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Invalid DID key format.");
    }

    [Fact]
    public void ResolveDidKey_ShouldFail_ForUnsupportedKeyType()
    {
        // Arrange
        var resolver = new DIDKeyResolver();
        var unsupportedDidKey = "did:key:z8iaMxQaDHjPHZtT4XWZemGXpMGv6PV3Xq2nXx57xBmWR"; // This is not an Ed25519 key

        // Act
        var result = resolver.ResolveDidKey(unsupportedDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "DID key is not a supported ED25519 key.");
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