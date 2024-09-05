using System.Text.Json.Nodes;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using FluentResults;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class DIDKeySignatureVerification
{
    private readonly ISha256Service _sha256Service;
    private readonly Ed25519SignatureVerification _ed25519Verifier;
    private readonly Secp256k1SignatureVerification _secp256k1Verifier;

    public DIDKeySignatureVerification(ISha256Service sha256Service)
    {
        _sha256Service = sha256Service;
        _ed25519Verifier = new Ed25519SignatureVerification();
        _secp256k1Verifier = new Secp256k1SignatureVerification();
    }

    public Result<ECheckSignatureResponse> VerifySignature(string credentialJson)
    {
        try
        {
            var credentialObject = JsonNode.Parse(credentialJson).AsObject();
            var proof = credentialObject["proof"].AsObject();
            var credentialWithoutProof = DIDKeyUtils.RemoveProofFromCredential(credentialObject);

            var issuerDid = credentialObject["issuer"]["id"].GetValue<string>();
            var proofValue = proof["proofValue"].GetValue<string>();

            var canonicalizedProof = DIDKeyUtils.CanonicalizeProof(proof);
            var proofHash = _sha256Service.HashData(System.Text.Encoding.UTF8.GetBytes(canonicalizedProof));

            var canonicalizedCredential = DIDKeyUtils.CanonicalizeCredential(credentialWithoutProof);
            var documentHash = _sha256Service.HashData(System.Text.Encoding.UTF8.GetBytes(canonicalizedCredential));

            var verifyData = DIDKeyUtils.CombineHashes(proofHash, documentHash);

            var publicKeyMultibase = DIDKeyUtils.ExtractPublicKeyMultibase(issuerDid);
            var publicKey = DIDKeyUtils.ConvertMultibaseToPublicKey(publicKeyMultibase);

            bool isValid;
            if (issuerDid.StartsWith("did:key:z6Mk")) // Ed25519
            {
                isValid = _ed25519Verifier.VerifySignature(verifyData, proofValue, publicKey);
            }
            else if (issuerDid.StartsWith("did:key:zQ3s")) // Secp256k1
            {
                isValid = _secp256k1Verifier.VerifySignature(verifyData, proofValue, publicKey);
            }
            else
            {
                return Result.Fail<ECheckSignatureResponse>("Unsupported DID key type");
            }

            return isValid
                ? Result.Ok(ECheckSignatureResponse.Valid)
                : Result.Ok(ECheckSignatureResponse.Invalid);
        }
        catch (Exception ex)
        {
            return Result.Fail<ECheckSignatureResponse>($"Error during DID Key signature verification: {ex.Message}");
        }
    }
}