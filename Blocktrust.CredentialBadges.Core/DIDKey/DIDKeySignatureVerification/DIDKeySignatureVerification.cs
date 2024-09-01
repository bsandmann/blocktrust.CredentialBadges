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

using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;

public class DIDKeySignatureVerification
{
    private readonly ISha256Service _sha256Service;

    public DIDKeySignatureVerification(ISha256Service sha256Service)
    {
        _sha256Service = sha256Service;
    }

    public Result<ECheckSignatureResponse> VerifySignature(string credentialJson)
    {
        try
        {
            var credentialObject = JsonNode.Parse(credentialJson).AsObject();

            // Extract the proof
            var proof = credentialObject["proof"].AsObject();
        
            // Create a copy of the credential without the proof
            var credentialWithoutProof = JsonSerializer.Deserialize<JsonObject>(credentialObject.ToJsonString());
            credentialWithoutProof.Remove("proof");

            var issuerDid = credentialObject["issuer"]["id"].GetValue<string>();
            var proofValue = proof["proofValue"].GetValue<string>();

            // Canonicalize and hash the proof
            var canonicalizedProof = CanonicalizeProof(proof);
            var proofHash = _sha256Service.HashData(Encoding.UTF8.GetBytes(canonicalizedProof));

            // Canonicalize and hash the credential without proof
            var canonicalizedCredential = CanonicalizeCredential(credentialWithoutProof);
            var documentHash = _sha256Service.HashData(Encoding.UTF8.GetBytes(canonicalizedCredential));

            // Combine the hashes
            var verifyData = CombineHashes(proofHash, documentHash);

            var publicKeyMultibase = ExtractPublicKeyMultibase(issuerDid);
            var publicKey = ConvertMultibaseToPublicKey(publicKeyMultibase);

            bool isValid = VerifySignatureInternal(verifyData, proofValue, publicKey);

            return isValid
                ? Result.Ok(ECheckSignatureResponse.Valid)
                : Result.Ok(ECheckSignatureResponse.Invalid);
        }
        catch (Exception ex)
        {
            return Result.Fail<ECheckSignatureResponse>($"Error during DID Key signature verification: {ex.Message}");
        }
    }

    public string CanonicalizeCredential(JsonObject credentialObject)
    {
        // Serialize the credential to a string
        var credentialString = credentialObject.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = false
        });

        // Parse JSON-LD to RDF Graph
        IGraph graph = new Graph();
        using (var reader = new StringReader(credentialString))
        {
            // Create a JSON-LD Parser instance
            var jsonLdParser = new JsonLdParser();

            // Create a handler for the graph
            var handler = new GraphHandler(graph);

            // Parse the JSON-LD string into the graph using the handler
            jsonLdParser.Load(handler, reader);
        }

        // Create a triple store and add the graph
        ITripleStore store = new TripleStore();
        store.Add(graph);

        // Canonicalize the RDF dataset
        var canonicalizer = new RdfCanonicalizer();
        var canonicalized = canonicalizer.Canonicalize(store);

        // Get the canonicalized N-Quads
        return canonicalized.SerializedNQuads;
    }
    
    
    public string CanonicalizeProof(JsonObject proof)
    {
        var proofCopy = JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(proof));
        proofCopy.Remove("proofValue");
    
        if (!proofCopy.ContainsKey("@context"))
        {
            proofCopy.Add("@context", "https://w3id.org/security/suites/ed25519-2020/v1");
        }
    
        return CanonicalizeCredential(proofCopy);
    }
    public byte[] CombineHashes(byte[] proofHash, byte[] documentHash)
    {
        byte[] combined = new byte[proofHash.Length + documentHash.Length];
        Buffer.BlockCopy(proofHash, 0, combined, 0, proofHash.Length);
        Buffer.BlockCopy(documentHash, 0, combined, proofHash.Length, documentHash.Length);
        return combined;
    }
    public string ExtractPublicKeyMultibase(string didKey)
    {
        return didKey.Substring(8);
    }

    public byte[] ConvertMultibaseToPublicKey(string multibaseKey)
    {
        if (string.IsNullOrEmpty(multibaseKey) || multibaseKey[0] != 'z')
        {
            throw new ArgumentException("Invalid multibase key. Must start with 'z'.", nameof(multibaseKey));
        }

        string encodedKey = multibaseKey.Substring(1);

        byte[] decodedBytes = Base58.Bitcoin.Decode(encodedKey).ToArray();

        return decodedBytes.Skip(2).ToArray();
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