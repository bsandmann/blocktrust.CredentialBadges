using FluentResults;
using SimpleBase;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class DIDKeyResolver
{
    private const string Ed25519Prefix = "z6Mk";

    public Result<string> ResolveDidKey(string didKey)
    {
        if (string.IsNullOrEmpty(didKey) || !didKey.StartsWith("did:key:"))
        {
            return Result.Fail("Invalid DID key format.");
        }

        var keyPart = didKey.Substring(8);
        if (string.IsNullOrEmpty(keyPart) || !keyPart.StartsWith(Ed25519Prefix))
        {
            return Result.Fail("DID key is not a supported ED25519 key.");
        }

        byte[] publicKeyBytes;
        try
        {
            publicKeyBytes = Base58.Bitcoin.Decode(keyPart).ToArray();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to decode base58 key: {ex.Message}");
        }

        if (publicKeyBytes.Length != 36)
        {
            return Result.Fail($"Invalid key length. Expected 36 bytes, got {publicKeyBytes.Length} bytes.");
        }

        // Extract the 32-byte public key, ignoring the first 2 bytes (multicodec prefix) and last 2 bytes
        var keyWithoutPrefix = publicKeyBytes.Skip(2).Take(32).ToArray();

        var verificationMethod = CreateVerificationMethod(didKey, keyWithoutPrefix);
        var keyAgreement = CreateKeyAgreement(didKey, keyWithoutPrefix);
        var didDocument = CreateDidDocument(didKey, verificationMethod, keyAgreement);

        var didDocumentJson = JsonSerializer.Serialize(didDocument, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        didDocumentJson = didDocumentJson.Replace("\"context\":", "\"@context\":");

        return Result.Ok(didDocumentJson);
    }

    private object CreateVerificationMethod(string didKey, byte[] publicKeyBytes)
    {
        return new
        {
            id = $"{didKey}#{didKey.Substring(8)}",
            type = "Ed25519VerificationKey2020",
            controller = didKey,
            publicKeyMultibase = didKey.Substring(8)
        };
    }

    private object CreateKeyAgreement(string didKey, byte[] ed25519PublicKey)
    {
        var x25519PublicKey = ConvertEd25519ToX25519(ed25519PublicKey);
        var x25519KeyId = Base58.Bitcoin.Encode(new byte[] { 0xec }.Concat(x25519PublicKey).ToArray());
            
        return new
        {
            id = $"{didKey}#z{x25519KeyId}",
            type = "X25519KeyAgreementKey2020",
            controller = didKey,
            publicKeyMultibase = $"z{x25519KeyId}"
        };
    }

    private object CreateDidDocument(string didKey, object verificationMethod, object keyAgreement)
    {
        var verificationMethodReference = $"{didKey}#{didKey.Substring(8)}";
        return new
        {
            context = new[] {
                "https://www.w3.org/ns/did/v1",
                "https://w3id.org/security/suites/ed25519-2020/v1",
                "https://w3id.org/security/suites/x25519-2020/v1"
            },
            id = didKey,
            verificationMethod = new[] { verificationMethod },
            authentication = new[] { verificationMethodReference },
            assertionMethod = new[] { verificationMethodReference },
            capabilityDelegation = new[] { verificationMethodReference },
            capabilityInvocation = new[] { verificationMethodReference },
            keyAgreement = new[] { keyAgreement }
        };
    }

    private byte[] ConvertEd25519ToX25519(byte[] ed25519PublicKey)
    {
        if (ed25519PublicKey.Length != 32)
        {
            throw new ArgumentException($"Invalid Ed25519 public key length. Expected 32 bytes, got {ed25519PublicKey.Length} bytes.");
        }

        // Implementation based on the algorithm described in RFC 7748
        BigInteger y = new BigInteger(ed25519PublicKey.Reverse().Concat(new byte[] { 0 }).ToArray());

        BigInteger p = BigInteger.Pow(2, 255) - 19;
        BigInteger A = 486662;

        BigInteger u = (y - 1) * ModInverse(y + 1, p) % p;

        byte[] result = u.ToByteArray().Reverse().Take(32).ToArray();
        if (result.Length < 32)
        {
            result = new byte[32 - result.Length].Concat(result).ToArray();
        }

        return result;
    }

    private BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m;
        BigInteger y = 0, x = 1;

        if (m == 1)
            return 0;

        while (a > 1)
        {
            BigInteger q = a / m;
            BigInteger t = m;

            m = a % m;
            a = t;
            t = y;

            y = x - q * y;
            x = t;
        }

        if (x < 0)
            x += m0;

        return x;
    }
}