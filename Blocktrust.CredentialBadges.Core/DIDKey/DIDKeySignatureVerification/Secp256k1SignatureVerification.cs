using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using SimpleBase;

namespace Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;

public class Secp256k1SignatureVerification
{
    public bool VerifySignature(byte[] message, string proofValue, byte[] publicKey)
    {
        try
        {
            byte[] signature = Base58.Bitcoin.Decode(proofValue.StartsWith("z") ? proofValue.Substring(1) : proofValue).ToArray();
            
            var curve = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            var q = curve.Curve.DecodePoint(publicKey);
            var publicKeyParameters = new ECPublicKeyParameters(q, domain);

            var signer = new ECDsaSigner();
            signer.Init(false, publicKeyParameters);

            var r = new Org.BouncyCastle.Math.BigInteger(1, signature, 0, 32);
            var s = new Org.BouncyCastle.Math.BigInteger(1, signature, 32, 32);

            return signer.VerifySignature(message, r, s);
        }
        catch (Exception)
        {
            return false;
        }
    }
}