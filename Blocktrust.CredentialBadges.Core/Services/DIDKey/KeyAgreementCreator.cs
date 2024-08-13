using SimpleBase;
using System.Numerics;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class KeyAgreementCreator : IKeyAgreementCreator
{
    public object CreateKeyAgreement(string didKey, byte[] ed25519PublicKey)
    {
        var x25519PublicKey = ConvertEd25519ToX25519(ed25519PublicKey);
        var x25519KeyId = Base58.Bitcoin.Encode(new byte[] { 0xec }.Concat(x25519PublicKey).ToArray());
            
        return new
        {
            id = $"{didKey}#z{x25519KeyId}",
            type = "X25519KeyAgreementKey2020",
            controller = didKey,
            publicKeyMultibase = $"z{x25519KeyId}"
        };
    }

    private byte[] ConvertEd25519ToX25519(byte[] ed25519PublicKey)
    {
        if (ed25519PublicKey.Length != 32)
        {
            throw new ArgumentException($"Invalid Ed25519 public key length. Expected 32 bytes, got {ed25519PublicKey.Length} bytes.");
        }

        BigInteger y = new BigInteger(ed25519PublicKey.Reverse().Concat(new byte[] { 0 }).ToArray());

        BigInteger p = BigInteger.Pow(2, 255) - 19;
        BigInteger A = 486662;

        BigInteger u = (y - 1) * ModInverse(y + 1, p) % p;

        byte[] result = u.ToByteArray().Reverse().Take(32).ToArray();
        if (result.Length < 32)
        {
            result = new byte[32 - result.Length].Concat(result).ToArray();
        }

        return result;
    }

    private BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m;
        BigInteger y = 0, x = 1;

        if (m == 1)
            return 0;

        while (a > 1)
        {
            BigInteger q = a / m;
            BigInteger t = m;

            m = a % m;
            a = t;
            t = y;

            y = x - q * y;
            x = t;
        }

        if (x < 0)
            x += m0;

        return x;
    }
}