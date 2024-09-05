using System.Text;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentAssertions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification.DidKeyVerification;

public class Ed25519SignatureVerificationTests
{
    private readonly Ed25519SignatureVerification _verifier;

    public Ed25519SignatureVerificationTests()
    {
        _verifier = new Ed25519SignatureVerification();
    }

    [Fact]
    public void VerifySignature_FullCycle_ReturnsTrue()
    {
        // Arrange
        var keyPair = GenerateEd25519KeyPair();
        var publicKey = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
        var privateKey = ((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded();

        byte[] message = Encoding.UTF8.GetBytes("Test message");
        byte[] signature = SignMessage(message, privateKey);
        string proofValue = "z" + Base58.Bitcoin.Encode(signature);

        // Act
        bool result = _verifier.VerifySignature(message, proofValue, publicKey);

        // Assert
        result.Should().BeTrue();
    }

    // Helper methods
    private AsymmetricCipherKeyPair GenerateEd25519KeyPair()
    {
        var keyPairGenerator = new Ed25519KeyPairGenerator();
        keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        return keyPairGenerator.GenerateKeyPair();
    }

    private byte[] SignMessage(byte[] message, byte[] privateKey)
    {
        var signer = new Ed25519Signer();
        signer.Init(true, new Ed25519PrivateKeyParameters(privateKey));
        signer.BlockUpdate(message, 0, message.Length);
        return signer.GenerateSignature();
    }
}