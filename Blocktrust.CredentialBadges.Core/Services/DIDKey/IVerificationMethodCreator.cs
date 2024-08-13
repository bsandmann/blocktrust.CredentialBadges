namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public interface IVerificationMethodCreator
{
    object CreateVerificationMethod(string didKey, byte[] publicKeyBytes);
}