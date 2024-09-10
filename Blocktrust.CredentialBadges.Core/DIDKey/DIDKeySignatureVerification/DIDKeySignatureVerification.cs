using System.Text.Json.Nodes;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using FluentResults;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class DIDKeySignatureVerification
{
    private readonly ISha256Service _sha256Service;
    private readonly IEcService _ecService;

    public DIDKeySignatureVerification(ISha256Service sha256Service, IEcService ecService)
    {
        _sha256Service = sha256Service;
        _ecService = ecService;
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
                isValid = VerifyEd25519Signature(verifyData, proofValue, publicKey);
            }
            else if (issuerDid.StartsWith("did:key:zQ3s")) // Secp256k1
            {
                byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
                isValid = _ecService.VerifyData(verifyData, signature, publicKey);
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

    private bool VerifyEd25519Signature(byte[] message, string proofValue, byte[] publicKey)
    {
        try
        {
            byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
            var verifier = new Ed25519Signer();
            verifier.Init(false, new Ed25519PublicKeyParameters(publicKey, 0));
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
        catch (Exception)
        {
            return false;
        }
    }
}