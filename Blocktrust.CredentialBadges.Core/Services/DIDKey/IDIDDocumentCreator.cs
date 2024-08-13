namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public interface IDidDocumentCreator
{
    object CreateDidDocument(string didKey, object verificationMethod, object keyAgreement);
}