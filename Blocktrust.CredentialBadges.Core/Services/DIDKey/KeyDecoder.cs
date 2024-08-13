using FluentResults;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class KeyDecoder : IKeyDecoder
{
    public Result<byte[]> DecodeKey(string didKey)
    {
        var keyPart = didKey.Substring(8);
        byte[] publicKeyBytes;

        try
        {
            publicKeyBytes = Base58.Bitcoin.Decode(keyPart).ToArray();
        }
        catch (Exception ex)
        {
            return Result.Fail<byte[]>($"Failed to decode base58 key: {ex.Message}");
        }

        if (publicKeyBytes.Length != 36)
        {
            return Result.Fail<byte[]>($"Invalid key length. Expected 36 bytes, got {publicKeyBytes.Length} bytes.");
        }

        return Result.Ok(publicKeyBytes.Skip(2).Take(32).ToArray());
    }
}