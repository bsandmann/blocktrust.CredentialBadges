namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public interface IKeyAgreementCreator
{
    object CreateKeyAgreement(string didKey, byte[] ed25519PublicKey);
}