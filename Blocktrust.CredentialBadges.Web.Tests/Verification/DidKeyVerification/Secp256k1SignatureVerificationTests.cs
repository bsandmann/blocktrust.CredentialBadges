using System.Text;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentAssertions;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification.DidKeyVerification;

public class Secp256k1SignatureVerificationTests
{
    private readonly Secp256k1SignatureVerification _verifier;

    public Secp256k1SignatureVerificationTests()
    {
        _verifier = new Secp256k1SignatureVerification();
    }

    [Fact]
    public void VerifySignature_FullCycle_ReturnsTrue()
    {
        // Arrange
        var keyPair = GenerateSecp256k1KeyPair();
        var publicKey = ((ECPublicKeyParameters)keyPair.Public).Q.GetEncoded(true);
        var privateKey = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned();

        byte[] message = Encoding.UTF8.GetBytes("Test message");
        byte[] signature = SignMessage(message, privateKey);
        string proofValue = "z" + Base58.Bitcoin.Encode(signature);

        // Act
        bool result = _verifier.VerifySignature(message, proofValue, publicKey);

        // Assert
        result.Should().BeTrue();
    }

    // Helper methods
    private AsymmetricCipherKeyPair GenerateSecp256k1KeyPair()
    {
        var curve = ECNamedCurveTable.GetByName("secp256k1");
        var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        var keyGenerationParameters = new ECKeyGenerationParameters(domainParams, new SecureRandom());
        var generator = new ECKeyPairGenerator();
        generator.Init(keyGenerationParameters);
        return generator.GenerateKeyPair();
    }

    private byte[] SignMessage(byte[] message, byte[] privateKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        var curve = ECNamedCurveTable.GetByName("secp256k1");
        var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
        var privKey = new ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(1, privateKey), domainParams);
        signer.Init(true, privKey);
        signer.BlockUpdate(message, 0, message.Length);
        return signer.GenerateSignature();
    }
}