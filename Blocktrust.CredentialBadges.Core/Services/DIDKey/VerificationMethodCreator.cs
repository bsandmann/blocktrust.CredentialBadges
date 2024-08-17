namespace Blocktrust.CredentialBadges.Core.Services.DIDKey
{
    public class VerificationMethodCreator : IVerificationMethodCreator
    {
        public object CreateVerificationMethod(string didKey, byte[] publicKeyBytes)
        {
            return new
            {
                id = $"{didKey}#{didKey.Substring(8)}",
                type = "Ed25519VerificationKey2020",
                controller = didKey,
                publicKeyMultibase = didKey.Substring(8)
            };
        }
    }
}