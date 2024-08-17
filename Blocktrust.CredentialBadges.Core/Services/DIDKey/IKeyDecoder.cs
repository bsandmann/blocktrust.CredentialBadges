using FluentResults;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public interface IKeyDecoder
{
    Result<byte[]> DecodeKey(string didKey);
}