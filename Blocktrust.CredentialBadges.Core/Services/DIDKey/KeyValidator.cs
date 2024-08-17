using FluentResults;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class KeyValidator : IKeyValidator
{
    private const string Ed25519Prefix = "z6Mk";

    public Result ValidateDidKey(string didKey)
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

        return Result.Ok();
    }
}