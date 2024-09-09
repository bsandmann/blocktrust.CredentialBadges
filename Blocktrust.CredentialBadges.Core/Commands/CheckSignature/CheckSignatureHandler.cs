using Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;
using Blocktrust.CredentialBadges.Core.Commands.CheckDIDKeySignature;
using FluentResults;
using MediatR;
using System.Text.Json;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

public class CheckSignatureHandler : IRequestHandler<CheckSignatureRequest, Result<ECheckSignatureResponse>>
{
    private readonly IMediator _mediator;

    public CheckSignatureHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<ECheckSignatureResponse>> Handle(CheckSignatureRequest request,
        CancellationToken cancellationToken)
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
                    var prismRequest = new CheckPrismDIDSignatureRequest(did, request.OpenBadgeCredential);
                    return await _mediator.Send(prismRequest, cancellationToken);
                case "key":
                    var credentialJson = JsonSerializer.Serialize(request.OpenBadgeCredential);
                    var didKeyRequest = new CheckDIDKeySignatureRequest(credentialJson);
                    return await _mediator.Send(didKeyRequest, cancellationToken);
                case "web":
                    return Result.Ok(ECheckSignatureResponse.UnsupportedDidMethod);
                default:
                    return Result.Ok(ECheckSignatureResponse.UnsupportedDidMethod);
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error during signature verification: {ex.Message}");
        }
    }
}