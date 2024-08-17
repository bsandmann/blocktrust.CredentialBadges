using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class KeyAgreementCreatorTests
{
    private readonly KeyAgreementCreator _keyAgreementCreator;

    public KeyAgreementCreatorTests()
    {
        _keyAgreementCreator = new KeyAgreementCreator();
    }

    [Theory]
    [InlineData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH")]
    public void CreateKeyAgreement_ShouldCreateValidKeyAgreement(string didKey)
    {
        // Arrange
        var ed25519PublicKey = new byte[32]; // Dummy public key bytes

        // Act
        var keyAgreement = _keyAgreementCreator.CreateKeyAgreement(didKey, ed25519PublicKey);

        // Assert
        keyAgreement.Should().NotBeNull();
        keyAgreement.GetType().GetProperty("id").GetValue(keyAgreement).As<string>().Should().StartWith(didKey);
        keyAgreement.GetType().GetProperty("type").GetValue(keyAgreement).Should().Be("X25519KeyAgreementKey2020");
        keyAgreement.GetType().GetProperty("controller").GetValue(keyAgreement).Should().Be(didKey);
        keyAgreement.GetType().GetProperty("publicKeyMultibase").GetValue(keyAgreement).As<string>().Should().StartWith("z");
    }
}