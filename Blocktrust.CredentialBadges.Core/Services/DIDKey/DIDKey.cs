using FluentResults;
using SimpleBase;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class DIDKeyResolver
{
    private static readonly Dictionary<string, (string, string, string)> MulticodecPrefixes = new()
    {
        { "z6Mk", ("JsonWebKey2020", "OKP", "Ed25519") },
        // Add other key types here if needed
    };

    public Result<string> ResolveDidKey(string didKey)
    {
        try
        {
            if (string.IsNullOrEmpty(didKey) || !didKey.StartsWith("did:key:"))
            {
                return Result.Fail("Invalid DID key format.");
            }

            var keyPart = didKey.Substring(8);
            if (string.IsNullOrEmpty(keyPart))
            {
                return Result.Fail("DID key is missing the public key part.");
            }

            var prefix = keyPart.Substring(0, 4);
            if (!MulticodecPrefixes.TryGetValue(prefix, out var keyInfo))
            {
                return Result.Fail($"Unsupported key type: {prefix}");
            }

            var (keyType, kty, crv) = keyInfo;

            byte[] publicKeyBytes;
            try
            {
                publicKeyBytes = Base58.Bitcoin.Decode(keyPart).ToArray();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Failed to decode base58 key: {ex.Message}");
            }

            // Remove multicodec prefix (0xed01 for Ed25519)
            publicKeyBytes = publicKeyBytes.Skip(2).ToArray();

            var verificationMethod = CreateVerificationMethod(didKey, keyType, kty, crv, publicKeyBytes);
            var didDocument = CreateDidDocument(didKey, verificationMethod);

            var didDocumentJson = JsonSerializer.Serialize(didDocument, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            return Result.Ok(didDocumentJson);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error occurred: {ex.Message}");
        }
    }

    private object CreateVerificationMethod(string didKey, string keyType, string kty, string crv, byte[] publicKeyBytes)
    {
        var id = $"{didKey}#{didKey.Substring(8)}";
        var controller = didKey;

        var jwk = new Dictionary<string, object>
        {
            { "kty", kty },
            { "crv", crv },
            { "x", Base64UrlEncode(publicKeyBytes) }
        };

        return new
        {
            id = id,
            type = keyType,
            controller = controller,
            publicKeyJwk = jwk
        };
    }

    private object CreateDidDocument(string didKey, object verificationMethod)
    {
        return new
        {
            context = new[] {
                "https://www.w3.org/ns/did/v1",
                "https://w3id.org/security/suites/jws-2020/v1"
            },
            id = didKey,
            verificationMethod = new[] { verificationMethod },
            assertionMethod = new[] { $"{didKey}#{didKey.Substring(8)}" },
            authentication = new[] { $"{didKey}#{didKey.Substring(8)}" },
            capabilityInvocation = new[] { $"{didKey}#{didKey.Substring(8)}" },
            capabilityDelegation = new[] { $"{didKey}#{didKey.Substring(8)}" }
        };
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}