using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class Secp256k1SignatureVerification
{
    private readonly ECDomainParameters _ecDomainParameters;

    public Secp256k1SignatureVerification()
    {
        var curve = SecNamedCurves.GetByName("secp256k1");
        _ecDomainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
    }

    public bool VerifySignature(byte[] message, string proofValue, byte[] publicKey)
    {
        try
        {
            byte[] signatureBytes = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();

            var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            var publicKeyParameters = new ECPublicKeyParameters("EC", _ecDomainParameters.Curve.DecodePoint(publicKey), _ecDomainParameters);

            signer.Init(false, publicKeyParameters);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.VerifySignature(signatureBytes);
        }
        catch (Exception)
        {
            return false;
        }
    }
}