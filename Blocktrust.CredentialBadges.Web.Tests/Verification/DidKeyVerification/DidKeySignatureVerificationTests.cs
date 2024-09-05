using System.Text.Json;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentAssertions;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification.DidKeyVerification;

public class DIDKeySignatureVerificationTests
{
    private readonly DIDKeySignatureVerification _verifier;

    public DIDKeySignatureVerificationTests()
    {
        var sha256Service = new Sha256ServiceBouncyCastle();
        _verifier = new DIDKeySignatureVerification(sha256Service);
    }

    [Fact]
    public void VerifySignature_ValidCredential_ReturnsValid()
    {
        // Arrange
        string credentialJson = TestDidKeyCredentials.ValidCredential;

        // Act
        var result = _verifier.VerifySignature(credentialJson);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Valid);
    }

    [Fact]
    public void VerifySignature_InvalidSignature_ReturnsInvalid()
    {
        // Arrange
        string credentialJson = TestDidKeyCredentials.InvalidSignatureCredential;

        // Act
        var result = _verifier.VerifySignature(credentialJson);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Invalid);
    }

    [Fact]
    public void VerifySignature_InvalidDIDKey_ReturnsFailure()
    {
        // Arrange
        string credentialJson = TestDidKeyCredentials.InvalidDIDKeyCredential;

        // Act
        var result = _verifier.VerifySignature(credentialJson);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("Error during DID Key signature verification");
    }

    [Fact]
    public void VerifySignature_InvalidJson_ReturnsFailure()
    {
        // Arrange
        string invalidJson = "{ invalid json }";

        // Act
        var result = _verifier.VerifySignature(invalidJson);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("Error during DID Key signature verification");
    }

    [Fact]
    public void VerifySignature_MissingFields_ReturnsFailure()
    {
        // Arrange
        var incompleteCredential = new
        {
            verifiableCredential = new[]
            {
                new
                {
                    issuer = new { },
                    proof = new { }
                }
            }
        };
        string credentialJson = JsonSerializer.Serialize(incompleteCredential);

        // Act
        var result = _verifier.VerifySignature(credentialJson);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle().Which.Message.Should().Contain("Error during DID Key signature verification");
    }
}