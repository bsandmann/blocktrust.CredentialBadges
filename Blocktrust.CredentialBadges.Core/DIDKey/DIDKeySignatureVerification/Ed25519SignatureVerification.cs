using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class Ed25519SignatureVerification
{
    public bool VerifySignature(byte[] message, string proofValue, byte[] publicKey)
    {
        try
        {
            byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
            var verifier = new Ed25519Signer();
            verifier.Init(false, new Ed25519PublicKeyParameters(publicKey, 0));
            verifier.BlockUpdate(message, 0, message.Length);
            return verifier.VerifySignature(signature);
        }
        catch (Exception)
        {
            return false;
        }
    }
}