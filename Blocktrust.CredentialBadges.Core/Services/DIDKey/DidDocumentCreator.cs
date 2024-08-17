namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class DidDocumentCreator : IDidDocumentCreator
{
    public object CreateDidDocument(string didKey, object verificationMethod, object keyAgreement)
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
}