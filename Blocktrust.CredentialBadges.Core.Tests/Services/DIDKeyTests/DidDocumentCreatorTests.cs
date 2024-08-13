using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;
using System.Text.Json;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class DidDocumentCreatorTests
{
    private readonly DidDocumentCreator _didDocumentCreator;

    public DidDocumentCreatorTests()
    {
        _didDocumentCreator = new DidDocumentCreator();
    }

    [Theory]
    [InlineData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH")]
    public void CreateDidDocument_ShouldCreateValidDidDocument(string didKey)
    {
        // Arrange
        var verificationMethod = new
        {
            id = $"{didKey}#{didKey.Substring(8)}",
            type = "Ed25519VerificationKey2020",
            controller = didKey,
            publicKeyMultibase = didKey.Substring(8)
        };

        var keyAgreement = new
        {
            id = $"{didKey}#z6LSbysY2xFMRmGGYnX7FnFDdPTrj3f7zbwpuRgkYdUZVBH",
            type = "X25519KeyAgreementKey2020",
            controller = didKey,
            publicKeyMultibase = "z6LSbysY2xFMRmGGYnX7FnFDdPTrj3f7zbwpuRgkYdUZVBH"
        };

        // Act
        var didDocument = _didDocumentCreator.CreateDidDocument(didKey, verificationMethod, keyAgreement);

        // Assert
        didDocument.Should().NotBeNull();
            
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(didDocument));

        // Check @context
        jsonElement.GetProperty("context").GetArrayLength().Should().Be(3);
        jsonElement.GetProperty("context")[0].GetString().Should().Be("https://www.w3.org/ns/did/v1");
        jsonElement.GetProperty("context")[1].GetString().Should().Be("https://w3id.org/security/suites/ed25519-2020/v1");
        jsonElement.GetProperty("context")[2].GetString().Should().Be("https://w3id.org/security/suites/x25519-2020/v1");

        // Check id
        jsonElement.GetProperty("id").GetString().Should().Be(didKey);

        // Check verificationMethod
        jsonElement.GetProperty("verificationMethod").GetArrayLength().Should().Be(1);
        var vm = jsonElement.GetProperty("verificationMethod")[0];
        vm.GetProperty("id").GetString().Should().Be(verificationMethod.id);
        vm.GetProperty("type").GetString().Should().Be(verificationMethod.type);
        vm.GetProperty("controller").GetString().Should().Be(verificationMethod.controller);
        vm.GetProperty("publicKeyMultibase").GetString().Should().Be(verificationMethod.publicKeyMultibase);

        // Check authentication, assertionMethod, capabilityDelegation, capabilityInvocation
        CheckVerificationRelationship(jsonElement, "authentication", verificationMethod.id);
        CheckVerificationRelationship(jsonElement, "assertionMethod", verificationMethod.id);
        CheckVerificationRelationship(jsonElement, "capabilityDelegation", verificationMethod.id);
        CheckVerificationRelationship(jsonElement, "capabilityInvocation", verificationMethod.id);

        // Check keyAgreement
        jsonElement.GetProperty("keyAgreement").GetArrayLength().Should().Be(1);
        var ka = jsonElement.GetProperty("keyAgreement")[0];
        ka.GetProperty("id").GetString().Should().Be(keyAgreement.id);
        ka.GetProperty("type").GetString().Should().Be(keyAgreement.type);
        ka.GetProperty("controller").GetString().Should().Be(keyAgreement.controller);
        ka.GetProperty("publicKeyMultibase").GetString().Should().Be(keyAgreement.publicKeyMultibase);
    }

    private void CheckVerificationRelationship(JsonElement jsonElement, string relationshipName, string expectedId)
    {
        jsonElement.GetProperty(relationshipName).GetArrayLength().Should().Be(1);
        jsonElement.GetProperty(relationshipName)[0].GetString().Should().Be(expectedId);
    }
}