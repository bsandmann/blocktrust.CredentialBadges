using Blocktrust.CredentialBadges.Core.Services.DIDPrism;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Prism;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;

public class CheckPrismDIDSignatureHandler : IRequestHandler<CheckPrismDIDSignatureRequest, Result<ECheckSignatureResponse>>
{
    private readonly ExtractPrismPubKeyFromLongFormDid _extractPrismPubKey;
    private readonly EcServiceBouncyCastle _ecService;

    public CheckPrismDIDSignatureHandler(ExtractPrismPubKeyFromLongFormDid extractPrismPubKey, EcServiceBouncyCastle ecService)
    {
        _extractPrismPubKey = extractPrismPubKey;
        _ecService = ecService;
    }

    public async Task<Result<ECheckSignatureResponse>> Handle(CheckPrismDIDSignatureRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var publicKey = _extractPrismPubKey.Extract(request.Did);

            var jwt = request.OpenBadgeCredential.Jwt;

            byte[] signatureBytes = PrismEncoding.Base64ToByteArray(jwt.Signature.Replace('-', '+').Replace('_', '/'));

            string signInInput = $"{PrismEncoding.ByteArrayToBase64(PrismEncoding.Utf8StringToByteArray($"{jwt.HeadersAsJson}"))}.{PrismEncoding.ByteArrayToBase64(PrismEncoding.Utf8StringToByteArray($"{jwt.PayloadAsJson}"))}";

            byte[] dataToVerify = PrismEncoding.Utf8StringToByteArray(signInInput);

            bool isValid = _ecService.VerifyDataWithoutDER(dataToVerify, signatureBytes, publicKey);

            return isValid 
                ? Result.Ok(ECheckSignatureResponse.Valid) 
                : Result.Ok(ECheckSignatureResponse.Invalid);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error during signature verification: {ex.Message}");
        }
    }
}