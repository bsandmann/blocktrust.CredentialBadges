using Blocktrust.CredentialBadges.Core.Services.DIDPrism;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Prism;
using FluentResults;
using MediatR;
using DidPrismResolverClient; // the PrismDidClient and related classes
using System.Net;
using System.Text.Json;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature
{
    public class CheckPrismDIDSignatureHandler : IRequestHandler<CheckPrismDIDSignatureRequest, Result<ECheckSignatureResponse>>
    {
        private readonly ExtractPrismPubKeyFromLongFormDid _extractPrismPubKey;
        private readonly PrismDidClient _prismDidClient;
        private readonly EcServiceBouncyCastle _ecService;

        public CheckPrismDIDSignatureHandler(
            ExtractPrismPubKeyFromLongFormDid extractPrismPubKey,
            EcServiceBouncyCastle ecService,
            PrismDidClient prismDidClient)
        {
            _extractPrismPubKey = extractPrismPubKey;
            _ecService = ecService;
            _prismDidClient = prismDidClient;
        }

        public async Task<Result<ECheckSignatureResponse>> Handle(CheckPrismDIDSignatureRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // 1) Basic check on DID method
                if (!request.Did.Contains("did:prism:", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Result.Fail("Invalid DID method (not did:prism:...)");
                }

                // 2) Identify short vs. long form
                var colonSplit = request.Did.Split(':', StringSplitOptions.RemoveEmptyEntries);
                bool isLongForm = (colonSplit.Length == 4);
                bool isShortForm = (colonSplit.Length == 3);

                if (!isLongForm && !isShortForm)
                {
                    return Result.Fail("Invalid DID structure. Must be short or long form of did:prism.");
                }

                // 3) Attempt to get public key
                byte[] publicKey;

                if (isLongForm)
                {
                    // a) Extract local key from the long form
                    var extractedPublicKey = _extractPrismPubKey.Extract(request.Did);

                    // b) Call the API to see if DID is deactivated or to confirm a (possibly updated) public key
                    var resolution = await TryResolvePublicKeyAsync(request.Did, cancellationToken);

                    if (resolution.IsSuccess)
                    {
                        // If the API call is successful, we override with the key from the DID Document
                        publicKey = resolution.PublicKey;
                    }
                    else
                    {
                        if (resolution.IsDeactivated)
                        {
                            // If DID is specifically marked as deactivated in the resolution response,
                            // we do not accept the local extracted key
                            return Result.Fail("DID is deactivated (per resolution API).");
                        }
                        else
                        {
                            // If the API is unreachable or fails in some other way,
                            // we ignore and keep the local extraction
                            publicKey = extractedPublicKey;
                        }
                    }
                }
                else
                {
                    // Short Form => Must always rely on the API
                    var resolution = await TryResolvePublicKeyAsync(request.Did, cancellationToken);

                    if (!resolution.IsSuccess)
                    {
                        if (resolution.IsDeactivated)
                        {
                            return Result.Fail("DID is deactivated (per resolution API).");
                        }
                        else
                        {
                            // If unreachable or any other resolution error => we fail for short form
                            return Result.Fail($"Failed to resolve short-form DID: {resolution.ErrorMessage}");
                        }
                    }

                    // If success => we have a valid key from the DID Document
                    publicKey = resolution.PublicKey;
                }

                // 4) We have a final 'publicKey' to use for verifying the signature
                var jwt = request.OpenBadgeCredential.Jwt;

                byte[] signatureBytes = PrismEncoding.Base64ToByteArray(
                    jwt.Signature
                        .Replace('-', '+')
                        .Replace('_', '/')
                );

                // Reconstruct the content that was signed:
                var signInput = $"{PrismEncoding.ByteArrayToBase64(PrismEncoding.Utf8StringToByteArray(jwt.HeadersAsJson))}"
                                + $".{PrismEncoding.ByteArrayToBase64(PrismEncoding.Utf8StringToByteArray(jwt.PayloadAsJson))}";

                byte[] dataToVerify = PrismEncoding.Utf8StringToByteArray(signInput);

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

        /// <summary>
        /// Attempts to resolve the DID via the PrismDidClient and extract the single assertionMethod's public key.
        /// Returns a small result object that distinguishes between:
        ///   - success (with public key),
        ///   - unreachable/unknown error,
        ///   - or "deactivated" if we discover that from the resolution API (e.g. via 410 or a special response).
        /// </summary>
        private async Task<DidResolutionKeyResult> TryResolvePublicKeyAsync(string did, CancellationToken cancellationToken)
        {
            try
            {
                // This call will throw PrismDidResolutionException if not 2xx
                var didDocument = await _prismDidClient.ResolveDidDocumentAsync(did, cancellationToken: cancellationToken);

                // If we get here, the resolution call returned a 2xx code. We now check the DID Document.
                if (didDocument == null)
                {
                    return DidResolutionKeyResult.Failure("Empty DID document returned from server.");
                }

                // If the server signals "deactivated" by returning an empty doc or some special field,
                // you can detect it here. For example, if a 410 is returned, you'd have caught it in the catch block.
                // Otherwise, check for assertionMethod as before:
                if (didDocument.AssertionMethod == null || didDocument.AssertionMethod.Count == 0)
                {
                    return DidResolutionKeyResult.Failure("No issuer key found in DID document.");
                }
                if (didDocument.AssertionMethod.Count > 1)
                {
                    return DidResolutionKeyResult.Failure("Multiple issuer keys found in DID document.");
                }

                var assertionMethod = didDocument.AssertionMethod[0].Split('#');
                if (assertionMethod.Length != 2)
                {
                    return DidResolutionKeyResult.Failure("Invalid AssertionMethod key syntax.");
                }

                var keyPart = assertionMethod[1];
                var issuingKey = didDocument.VerificationMethod.FirstOrDefault(
                    p => p.Id == didDocument.AssertionMethod[0]
                      || p.Id == keyPart
                      || p.Id == "#" + keyPart);

                if (issuingKey?.PublicKeyJwk == null)
                {
                    return DidResolutionKeyResult.Failure("Issuer key not found in DID document.");
                }

                if (!issuingKey.PublicKeyJwk.KeyType.Equals("EC", StringComparison.OrdinalIgnoreCase))
                {
                    return DidResolutionKeyResult.Failure("Unable to extract public key (non-EC key) from DID document.");
                }

                // Finally parse out x & y
                var xKey = issuingKey.PublicKeyJwk.X;
                var yKey = issuingKey.PublicKeyJwk.Y;

                if (string.IsNullOrEmpty(xKey))
                {
                    return DidResolutionKeyResult.Failure("Missing 'x' coordinate in public key JWK.");
                }

                byte[] finalKey;
                if (string.IsNullOrEmpty(yKey))
                {
                    // Sometimes y isn't provided if it's a compressed key?
                    finalKey = PrismEncoding.HexToByteArray(xKey);
                }
                else
                {
                    var xKeyBytes = PrismEncoding.Base64ToByteArray(xKey);
                    var yKeyBytes = PrismEncoding.Base64ToByteArray(yKey);
                    finalKey = PrismEncoding.HexToByteArray(
                        PrismEncoding.PublicKeyPairByteArraysToHex(xKeyBytes, yKeyBytes)
                    );
                }

                return DidResolutionKeyResult.Success(finalKey);
            }
            catch (PrismDidResolutionException ex)
            {
                // The PrismDidResolutionException might contain e.g. 404 or 410 or 500 in the ex.Message/body.
                // For example, if you want to interpret a 410 Gone as "DID is deactivated," do so here:
                if (ex.Message.Contains("410")) // or parse ex for specific status code
                {
                    return DidResolutionKeyResult.Deactivated("DID is deactivated (HTTP 410).");
                }

                // Otherwise, treat it as a generic failure/unreachable:
                return DidResolutionKeyResult.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                // Some other network or parsing error
                return DidResolutionKeyResult.Failure($"Unexpected error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// A small DTO/record to hold the outcome of a DID Document resolution & public key extraction.
    /// This helps simplify our branching logic.
    /// </summary>
    public record DidResolutionKeyResult
    {
        public bool IsSuccess { get; init; }
        public bool IsDeactivated { get; init; }
        public string ErrorMessage { get; init; } = string.Empty;
        public byte[] PublicKey { get; init; } = Array.Empty<byte>();

        // For success
        public static DidResolutionKeyResult Success(byte[] key) => new()
        {
            IsSuccess = true,
            IsDeactivated = false,
            PublicKey = key
        };

        // For generic failure (unreachable, invalid doc, etc.)
        public static DidResolutionKeyResult Failure(string error) => new()
        {
            IsSuccess = false,
            IsDeactivated = false,
            ErrorMessage = error
        };

        // For "deactivated" specifically
        public static DidResolutionKeyResult Deactivated(string error) => new()
        {
            IsSuccess = false,
            IsDeactivated = true,
            ErrorMessage = error
        };
    }
}
