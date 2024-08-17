namespace Blocktrust.CredentialBadges.Core.Tests;

using OpenPrismNode;
using Prism;

public class PrismLongformDecoderTests
{
    [Fact]
    public void Test1()
    {
        // var prismLongform = """did:prism:7e2dc793804699ea6c11570f02fb43eb32f9066f540d2b173209481b958c8504:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiECHMWgUM3HbC8942TkwHyBulPNiE-JaVdO2uO4Vp5fREASOwoHbWFzdGVyMBABSi4KCXNlY3AyNTZrMRIhAlUNGDTlTX9ivHD_6Kiv1dxtZzdEpCO8fTDBewycsaEw""";
        var prismLongform = """did:prism:9c96389a50a41e1bb0dac7e786cc646c33f57f514ae96b6375e0b56ff505ecc2:CuoDCucDElwKB21hc3RlcjAQAUJPCglzZWNwMjU2azESIO0iXdL6ojqCWrc0DnBrIzDWxDWHvJZJrFUpVseRqCBOGiCApaxtvKDF_ALngxfK9N_f2gSidw56qJuSVfDj8fqhTRJaCgVrZXktMRAEQk8KCXNlY3AyNTZrMRIg2QTlMsT1OedRhBUyGZD2Af7URrLdVkYrgiD0vEgQ71UaIECbLdQrydrNmDDUSWECBQZB_SVG8Keuq5AFzsqrflzNEloKBWtleS0yEANCTwoJc2VjcDI1NmsxEiDxNQzt9W-up3xZhM4M9IQ2uv66cmIzOYbekJ-zT30QDBogjmVepI63oZ_BFtKK_CHC1ioAo68ipTxXoa4opAHv9GISWgoFa2V5LTMQAkJPCglzZWNwMjU2azESIPfkwFHFPQssG0U7i3h2EMQsVtnTznRpwFR1eNd9J3tAGiAPXXJGCXDBkJsWcrAeGS7TkDDjXmRLnhbpu6BUiqFF7RpzCk9kaWQ6cHJpc206OWM5NjM4OWE1MGE0MWUxYmIwZGFjN2U3ODZjYzY0NmMzM2Y1N2Y1MTRhZTk2YjYzNzVlMGI1NmZmNTA1ZWNjMjp0ZXN0Eg1MaW5rZWREb21haW5zGhFodHRwczovL3Rlc3QuY29tLw""";

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
        
        //to base64
        var base64PublicKey = Convert.ToBase64String(publicKey);
        

        
    }
}