using FluentResults;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.Core.Services.DIDKey;

public class DIDKeyResolver
{
    private readonly IKeyValidator _keyValidator;
    private readonly IKeyDecoder _keyDecoder;
    private readonly IVerificationMethodCreator _verificationMethodCreator;
    private readonly IKeyAgreementCreator _keyAgreementCreator;
    private readonly IDidDocumentCreator _didDocumentCreator;

    public DIDKeyResolver(
        IKeyValidator keyValidator,
        IKeyDecoder keyDecoder,
        IVerificationMethodCreator verificationMethodCreator,
        IKeyAgreementCreator keyAgreementCreator,
        IDidDocumentCreator didDocumentCreator)
    {
        _keyValidator = keyValidator;
        _keyDecoder = keyDecoder;
        _verificationMethodCreator = verificationMethodCreator;
        _keyAgreementCreator = keyAgreementCreator;
        _didDocumentCreator = didDocumentCreator;
    }

    public Result<string> ResolveDidKey(string didKey)
    {
        var validationResult = _keyValidator.ValidateDidKey(didKey);
        if (validationResult.IsFailed)
        {
            return Result.Fail<string>(validationResult.Errors);
        }

        var decodedKeyResult = _keyDecoder.DecodeKey(didKey);
        if (decodedKeyResult.IsFailed)
        {
            return Result.Fail<string>(decodedKeyResult.Errors);
        }

        var publicKeyBytes = decodedKeyResult.Value;
        var verificationMethod = _verificationMethodCreator.CreateVerificationMethod(didKey, publicKeyBytes);
        var keyAgreement = _keyAgreementCreator.CreateKeyAgreement(didKey, publicKeyBytes);
        var didDocument = _didDocumentCreator.CreateDidDocument(didKey, verificationMethod, keyAgreement);

        var didDocumentJson = JsonSerializer.Serialize(didDocument, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        didDocumentJson = didDocumentJson.Replace("\"context\":", "\"@context\":");

        return Result.Ok(didDocumentJson);
    }
}