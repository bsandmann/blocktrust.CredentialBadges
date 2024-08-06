namespace Blocktrust.CredentialBadges.Core.Tests;

using OpenPrismNode;
using Prism;

public class PrismLongformDecoderTests
{
    [Fact]
    public void Test1()
    {
        var prismLongform = """did:prism:7e2dc793804699ea6c11570f02fb43eb32f9066f540d2b173209481b958c8504:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiECHMWgUM3HbC8942TkwHyBulPNiE-JaVdO2uO4Vp5fREASOwoHbWFzdGVyMBABSi4KCXNlY3AyNTZrMRIhAlUNGDTlTX9ivHD_6Kiv1dxtZzdEpCO8fTDBewycsaEw""";
        var longFormBase64 = prismLongform.Split(':')[3];
        var bytes = PrismEncoding.Base64ToByteArray(longFormBase64);
        var atalaOperation = AtalaOperation.Parser.ParseFrom(bytes);

        byte[] x = new byte[] { };
        byte[] y = new byte[] { };
        var issuingKey = atalaOperation.CreateDid.DidData.PublicKeys.First(p => p.Usage == KeyUsage.IssuingKey);

        var prismPublicKey = new PrismPublicKey(
            keyUsage: Enum.Parse<PrismKeyUsage>(issuingKey.Usage.ToString()),
            keyId: issuingKey.Id,
            curve: issuingKey.EcKeyData is not null ? issuingKey.EcKeyData.Curve : issuingKey.CompressedEcKeyData.Curve,
            keyX: issuingKey.KeyDataCase == PublicKey.KeyDataOneofCase.EcKeyData ? issuingKey.EcKeyData?.X.ToByteArray() :
            issuingKey.KeyDataCase == PublicKey.KeyDataOneofCase.CompressedEcKeyData ? PrismPublicKey.Decompress(PrismEncoding.ByteStringToByteArray(issuingKey.CompressedEcKeyData.Data), issuingKey.CompressedEcKeyData.Curve).Value.Item1 : null,
            keyY: issuingKey.KeyDataCase == PublicKey.KeyDataOneofCase.EcKeyData ? issuingKey.EcKeyData?.Y.ToByteArray() :
            issuingKey.KeyDataCase == PublicKey.KeyDataOneofCase.CompressedEcKeyData ? PrismPublicKey.Decompress(PrismEncoding.ByteStringToByteArray(issuingKey.CompressedEcKeyData.Data), issuingKey.CompressedEcKeyData.Curve).Value.Item2 : null);

        var publicKey = prismPublicKey.LongByteArray;
    }
}