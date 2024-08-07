using Blocktrust.CredentialBadges.Core.Prism;
using OpenPrismNode;
using FluentResults;
using MediatR;
using Blocktrust.CredentialBadges.Core.Crypto;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

public class CheckSignatureHandler : IRequestHandler<CheckSignatureRequest, Result<ECheckSignatureResponse>>
{
    public async Task<Result<ECheckSignatureResponse>> Handle(CheckSignatureRequest request, CancellationToken cancellationToken)
    {
        string? did = request?.OpenBadgeCredential?.Issuer?.Id?.ToString();
            
        if (string.IsNullOrWhiteSpace(did))
        {
            return Result.Fail("Issuer DID is missing");
        }

        string didMethod = did.Split(":")[1];
            
        try
        {
            switch (didMethod)
            {
                case "prism":
                    return CheckPrismDIDSignature(did, request);
                case "key":
                    // Implement key DID method
                    return Result.Fail("Key DID method not implemented");
                case "web":
                    // Implement web DID method
                    return Result.Fail("Web DID method not implemented");
                default:
                    return Result.Fail("Unsupported DID method");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error during signature verification: {ex.Message}");
        }
    }

    private Result<ECheckSignatureResponse> CheckPrismDIDSignature(string did, CheckSignatureRequest request)
    {
        bool isLongForm = did.Split(':').Length > 3;

        if (!isLongForm)
        {
            return Result.Fail("Short-form DID resolution is not implemented");
        }

        string encodedPKeys = did.Split(":")[3];
        var bytes = PrismEncoding.Base64ToByteArray(encodedPKeys);
        var atalaOperation = AtalaOperation.Parser.ParseFrom(bytes);

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

        string signature = request?.OpenBadgeCredential.Jwt.Signature;
        byte[] signatureBytes = PrismEncoding.Base64ToByteArray(signature);

        string payload = request?.OpenBadgeCredential.Jwt.PayloadAsJson;
        byte[] payloadBytes = PrismEncoding.Utf8StringToByteArray(payload);

        var ecService = new EcServiceBouncyCastle();

        bool isValid;
        if (ecService.IsValidDerSignature(signatureBytes))
        {
            isValid = ecService.VerifyData(payloadBytes, signatureBytes, publicKey);
        }
        else
        {
            // If the signature is not in DER format, convert it to DER before verification
            byte[] derSignature = ecService.ConvertToDerSignature(signatureBytes);
            isValid = ecService.VerifyData(payloadBytes, derSignature, publicKey);
        }

        return isValid 
            ? Result.Ok(ECheckSignatureResponse.Valid) 
            : Result.Fail("Signature verification failed");
    }
}