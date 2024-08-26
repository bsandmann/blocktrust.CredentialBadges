using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using Blocktrust.CredentialBadges.Core.Prism;
using FluentAssertions;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

public class DidKeySignatureVerificationTests
{
    private readonly EcServiceBouncyCastle _ecService;
    private readonly DIDKeySignatureVerification _verifier;

    public DidKeySignatureVerificationTests()
    {
        _ecService = new EcServiceBouncyCastle();
        _verifier = new DIDKeySignatureVerification(_ecService);
    }

    [Fact]
    public void VerifySignature_ValidSignature_ReturnsValid()
    {
        
        // Arrange
        string didKey = "did:key:z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";
        string proofValue = "zxFfvBhwcFa99uLFaJgJ3VYFfomD5qQgpb6vvKR2TgRjHbB4WcCS8mLfvNdu9WrDUTt1m6xZHVc7Cjux5RkNynfc";
        string signedData = """
                            {
                              "@context": [
                                "https://www.w3.org/2018/credentials/v1",
                                "https://purl.imsglobal.org/spec/ob/v3p0/context-3.0.1.json",
                                "https://w3id.org/security/suites/ed25519-2020/v1"
                              ],
                              "id": "urn:uuid:cebd27cf-f753-471d-bc2b-b728e51595f3",
                              "type": [
                                "VerifiableCredential",
                                "OpenBadgeCredential"
                              ],
                              "name": "DCC Test Credential",
                              "issuer": {
                                "type": [
                                  "Profile"
                                ],
                                "id": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                "name": "Digital Credentials Consortium Test Issuer",
                                "url": "https://dcconsortium.org",
                                "image": "https://user-images.githubusercontent.com/947005/133544904-29d6139d-2e7b-4fe2-b6e9-7d1022bb6a45.png"
                              },
                              "issuanceDate": "2023-08-02T21:19:28.154Z",
                              "credentialSubject": {
                                "type": [
                                  "AchievementSubject"
                                ],
                                "achievement": {
                                  "id": "urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922",
                                  "type": [
                                    "Achievement"
                                  ],
                                  "achievementType": "Diploma",
                                  "name": "Badge",
                                  "description": "This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.",
                                  "criteria": {
                                    "type": "Criteria",
                                    "narrative": "This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project"
                                  },
                                  "image": {
                                    "id": "https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png",
                                    "type": "Image"
                                  }
                                },
                                "name": "Jane Doe"
                              },
                              "expirationDate": "2025-07-26T00:00:00.000Z",
                              "proof": {
                                "type": "Ed25519Signature2020",
                                "created": "2023-08-02T21:19:28Z",
                                "verificationMethod": "did:key:z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC#z6MkhVTX9BF3NGYX6cc7jWpbNnR7cAjH8LUffabZP8Qu4ysC",
                                "proofPurpose": "assertionMethod",
                                "proofValue": "z6sVGrHrDE1K8TLSV8qK87GZEpNHH1S3TTi9KhKyKiXCDPtxT2Y2Hs5xX5ZK3McwhHGoGUdoG9tu9vJsLxMDazVC"
                              }
                            }
                            """;

        // Act
        
        var result = _verifier.VerifySignature(didKey, proofValue, signedData);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Valid);
    }

    [Fact]
    public void VerifySignature_InvalidSignature_ReturnsInvalid()
    {
        // Arrange
        string didKey = "did:key:z6Mktpn6cXks1PBKLMgZH2VaahvCtBMF6K8eCa7HzrnuYLZv";
        string proofValue = "InvalidSignature";
        string signedData = "SignedData";

        // Act
        var result = _verifier.VerifySignature(didKey, proofValue, signedData);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Invalid);
    }

    [Fact]
    public void VerifySignature_InvalidDIDKey_ReturnsFailure()
    {
        // Arrange
        string didKey = "did:key:InvalidDIDKey";
        string proofValue = "zxFfvBhwcFa99uLFaJgJ3VYFfomD5qQgpb6vvKR2TgRjHbB4WcCS8mLfvNdu9WrDUTt1m6xZHVc7Cjux5RkNynfc";
        string signedData = "SignedData";

        // Act
        var result = _verifier.VerifySignature(didKey, proofValue, signedData);

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
}