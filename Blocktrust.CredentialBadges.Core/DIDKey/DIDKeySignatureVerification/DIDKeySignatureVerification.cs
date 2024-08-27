using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using FluentResults;
using SimpleBase;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class DIDKeySignatureVerification
{
    private readonly EcServiceBouncyCastle _ecService;

    public DIDKeySignatureVerification(EcServiceBouncyCastle ecService)
    {
        _ecService = ecService;
    }

    public Result<ECheckSignatureResponse> VerifySignature(string credentialJson)
    {
        try
        {
            var credentialObject = JsonNode.Parse(credentialJson).AsObject();
            var verifiableCredential = credentialObject["verifiableCredential"][0].AsObject();
            var issuerDid = verifiableCredential["issuer"]["id"].GetValue<string>();
            var proofValue = verifiableCredential["proof"]["proofValue"].GetValue<string>();

            // Create a new object without the proof
            var credentialWithoutProof = new JsonObject();
            foreach (var property in verifiableCredential.AsObject())
            {
                if (property.Key != "proof")
                {
                    credentialWithoutProof.Add(property.Key, property.Value);
                }
            }

            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(credentialWithoutProof));

            var publicKeyMultibase = ExtractPublicKeyMultibase(issuerDid);
            var publicKey = ConvertMultibaseToPublicKey(publicKeyMultibase);
                
            bool isValid = VerifySignatureInternal(message, proofValue, publicKey);

            return isValid 
                ? Result.Ok(ECheckSignatureResponse.Valid) 
                : Result.Ok(ECheckSignatureResponse.Invalid);
        }
        catch (Exception ex)
        {
            return Result.Fail<ECheckSignatureResponse>($"Error during DID Key signature verification: {ex.Message}");
        }
    }

    public string ExtractPublicKeyMultibase(string didKey)
    {
        return didKey.Substring(8);
    }

    public byte[] ConvertMultibaseToPublicKey(string multibaseKey)
    {
        string encodedKey = multibaseKey.Substring(1);
        byte[] decodedKey = Base58.Bitcoin.Decode(encodedKey).ToArray();
        return decodedKey.Skip(2).ToArray();
    }

    public bool VerifySignatureInternal(byte[] message, string proofValue, byte[] publicKey)
    {
        try
        {
            byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
            return VerifyEd25519Signature(message, signature, publicKey);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool VerifyEd25519Signature(byte[] message, byte[] signature, byte[] publicKey)
    {
        try
        {
            var verifier = new Ed25519Signer();
            verifier.Init(false, new Ed25519PublicKeyParameters(publicKey));
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
        catch (Exception)
        {
            return false;
        }
    }
}