using FluentResults;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public interface IKeyValidator
{
    Result ValidateDidKey(string didKey);
}