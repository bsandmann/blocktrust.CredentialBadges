using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class VerificationMethodCreatorTests
{
    private readonly VerificationMethodCreator _verificationMethodCreator;

    public VerificationMethodCreatorTests()
    {
        _verificationMethodCreator = new VerificationMethodCreator();
    }

    [Theory]
    [InlineData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH")]
    public void CreateVerificationMethod_ShouldCreateValidVerificationMethod(string didKey)
    {
        // Arrange
        var publicKeyBytes = new byte[32]; // Dummy public key bytes

        // Act
        var verificationMethod = _verificationMethodCreator.CreateVerificationMethod(didKey, publicKeyBytes);

        // Assert
        verificationMethod.Should().NotBeNull();
        verificationMethod.GetType().GetProperty("id").GetValue(verificationMethod).Should().Be($"{didKey}#{didKey.Substring(8)}");
        verificationMethod.GetType().GetProperty("type").GetValue(verificationMethod).Should().Be("Ed25519VerificationKey2020");
        verificationMethod.GetType().GetProperty("controller").GetValue(verificationMethod).Should().Be(didKey);
        verificationMethod.GetType().GetProperty("publicKeyMultibase").GetValue(verificationMethod).Should().Be(didKey.Substring(8));
    }
}