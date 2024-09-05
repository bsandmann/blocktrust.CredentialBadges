using System.Text.Json;
using System.Text.Json.Nodes;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public static class DIDKeyUtils
{
    public static JsonObject RemoveProofFromCredential(JsonObject credentialObject)
    {
        var credentialWithoutProof = JsonSerializer.Deserialize<JsonObject>(credentialObject.ToJsonString());
        credentialWithoutProof.Remove("proof");
        return credentialWithoutProof;
    }

    public static string CanonicalizeCredential(JsonObject credentialObject)
    {
        var credentialString = credentialObject.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        IGraph graph = new Graph();
        using (var reader = new StringReader(credentialString))
        {
            var jsonLdParser = new JsonLdParser();
            var handler = new GraphHandler(graph);
            jsonLdParser.Load(handler, reader);
        }
        ITripleStore store = new TripleStore();
        store.Add(graph);
        var canonicalizer = new RdfCanonicalizer();
        var canonicalized = canonicalizer.Canonicalize(store);
        return canonicalized.SerializedNQuads;
    }

    public static string CanonicalizeProof(JsonObject proof)
    {
        var proofCopy = JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(proof));
        proofCopy.Remove("proofValue");
        if (!proofCopy.ContainsKey("@context"))
        {
            proofCopy.Add("@context", "https://w3id.org/security/suites/ed25519-2020/v1");
        }
        return CanonicalizeCredential(proofCopy);
    }

    public static byte[] CombineHashes(byte[] proofHash, byte[] documentHash)
    {
        byte[] combined = new byte[proofHash.Length + documentHash.Length];
        Buffer.BlockCopy(proofHash, 0, combined, 0, proofHash.Length);
        Buffer.BlockCopy(documentHash, 0, combined, proofHash.Length, documentHash.Length);
        return combined;
    }

    public static string ExtractPublicKeyMultibase(string didKey)
    {
        return didKey.Substring(8);
    }

    public static byte[] ConvertMultibaseToPublicKey(string multibaseKey)
    {
        if (string.IsNullOrEmpty(multibaseKey) || multibaseKey[0] != 'z')
        {
            throw new ArgumentException("Invalid multibase key. Must start with 'z'.", nameof(multibaseKey));
        }
        string encodedKey = multibaseKey.Substring(1);
        byte[] decodedBytes = SimpleBase.Base58.Bitcoin.Decode(encodedKey).ToArray();
        return decodedBytes.Skip(2).ToArray();
    }
}