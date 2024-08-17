using FluentAssertions;
using Blocktrust.CredentialBadges.Core.Services.DIDKey;

namespace Blocktrust.CredentialBadges.Core.Tests.Services.DIDKeyTests;

public class KeyValidatorTests
{
    private readonly KeyValidator _keyValidator;

    public KeyValidatorTests()
    {
        _keyValidator = new KeyValidator();
    }

    [Theory]
    [InlineData("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:z6Mkfriq1MqLBoPWecGoDLjguo1sB9brj6wT3qZ5BxkKpuP6")]
    [InlineData("did:key:z6MksQ35B5bwZDQq4QKuhQW2Sv6dcqwg4PqcSFf67pdgrtjB")]
    [InlineData("did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH")]
    public void ValidateDidKey_ShouldSucceed_ForValidDidKeys(string didKey)
    {
        // Act
        var result = _keyValidator.ValidateDidKey(didKey);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:invalid:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK")]
    [InlineData("did:key:invalidprefix")]
    public void ValidateDidKey_ShouldFail_ForInvalidDidKeys(string invalidDidKey)
    {
        // Act
        var result = _keyValidator.ValidateDidKey(invalidDidKey);

        // Assert
        result.IsFailed.Should().BeTrue();
    }
}