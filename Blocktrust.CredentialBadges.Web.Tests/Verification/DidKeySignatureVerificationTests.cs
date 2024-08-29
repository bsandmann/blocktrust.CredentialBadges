using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentAssertions;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Text;
using SimpleBase;
using Org.BouncyCastle.Crypto.Signers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

public class DidKeySignatureVerificationTests
{
    private readonly EcServiceBouncyCastle _ecService;
    private readonly DIDKeySignatureVerification _verifier;
    private readonly ISha256Service _sha256Service;

    public DidKeySignatureVerificationTests()
    {
        _ecService = new EcServiceBouncyCastle();
        _sha256Service = new Sha256ServiceBouncyCastle();
        _verifier = new DIDKeySignatureVerification(_sha256Service);
    }
    
       [Fact]
        public void CanonicalizeAndHashCredential_ValidCredential_ReturnsExpectedHash()
        {
            // Arrange
            var credentialWithoutProof = TestDidKeyCredentials.ValidCredentialWithoutProof;
            var credentialObject = JsonNode.Parse(credentialWithoutProof).AsObject();
    
            // Expected hash (placeholder)
            var expectedHash = "PLACE_EXPECTED_HASH_HERE";
    
            // Act
            var canonicalized = _verifier.CanonicalizeCredential(credentialObject);
            
            var hashedMessage = _sha256Service.HashData(Encoding.UTF8.GetBytes(canonicalized));
            
            var hashHex = Convert.ToHexString(hashedMessage);
    
            // Assert
            hashedMessage.Should().BeEquivalentTo(Convert.FromHexString(expectedHash));
        }

    [Fact]
    public void VerifySignatureInternal_FullCycle_ReturnsTrue()
    {
        // Arrange
        var keyPair = GenerateEd25519KeyPair();
        var publicKey = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
        var privateKey = ((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded();

        string didKey = CreateDidKeyFromPublicKey(publicKey);
        byte[] message = "Test message"u8.ToArray();
        byte[] signature = SignMessage(message, privateKey);
        string proofValue = "z" + Base58.Bitcoin.Encode(signature);

        string publicKeyMultibase = didKey.Substring(8); // Remove "did:key:" prefix
        byte[] convertedPublicKey = _verifier.ConvertMultibaseToPublicKey(publicKeyMultibase);

        // Act
        bool result = _verifier.VerifySignatureInternal(message, proofValue, convertedPublicKey);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyEd25519Signature_FullCycle_ReturnsTrue()
    {
        // Arrange
        var keyPair = GenerateEd25519KeyPair();
        var publicKey = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
        var privateKey = ((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded();

        byte[] message = Encoding.UTF8.GetBytes("Test message");
        byte[] signature = SignMessage(message, privateKey);

        // Act
        bool result = _verifier.VerifyEd25519Signature(message, signature, publicKey);

        // Assert
        result.Should().BeTrue();
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
    public void ExtractPublicKeyMultibase_ValidDIDKey_ReturnsCorrectMultibase()
    {
        // Arrange
        string didKey = "did:key:z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";

        // Act
        var result = _verifier.ExtractPublicKeyMultibase(didKey);

        // Assert
        result.Should().Be("z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv");
    }

    [Fact]
    public void ConvertMultibaseToPublicKey_ValidMultibase_ReturnsCorrectPublicKey()
    {
        // Arrange
        string multibase = "z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";

        // Act
        var result = _verifier.ConvertMultibaseToPublicKey(multibase);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(32); // Ed25519 public key length
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

    private string CreateDidKeyFromPublicKey(byte[] publicKey)
    {
        var multicodecPublicKey = new byte[] { 0xed, 0x01 }.Concat(publicKey).ToArray();
        var multibasePublicKey = "z" + Base58.Bitcoin.Encode(multicodecPublicKey);
        return $"did:key:{multibasePublicKey}";
    }
}