namespace Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;

using CheckRevocationStatus;
using CheckSignature;
using CheckTrustRegistry;
using FluentResults;
using MediatR;

public class VerifyOpenBadgeHandler : IRequestHandler<VerifyOpenBadgeRequest, Result<VerifyOpenBadgeResponse>>
{
    private readonly IMediator _mediator;

    public VerifyOpenBadgeHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<VerifyOpenBadgeResponse>> Handle(VerifyOpenBadgeRequest request, CancellationToken cancellationToken)
    {
        var verificationResult = new VerifyOpenBadgeResponse();
        var checkDate = DateTime.UtcNow;
        verificationResult.CheckedOn = checkDate;

        // TODO First do the Signature check
        var checkSignatureResult = await _mediator.Send(new CheckSignatureRequest(request.OpenBadgeCredential), cancellationToken);
        if (checkSignatureResult.IsFailed)
        {
            return checkSignatureResult.ToResult();
        }

        verificationResult.SignatureIsValid = checkSignatureResult.Value == ECheckSignatureResponse.Valid || checkSignatureResult.Value == ECheckSignatureResponse.UnsupportedDidMethod;
        

        // Then check the expiration & Issuance Date
        verificationResult.CredentialIssuanceDateIsNotInFuture = request.OpenBadgeCredential.ValidFrom < checkDate;

        if (request.OpenBadgeCredential.ValidUntil is not null)
        {
            verificationResult.CredentialIsNotExpired = request.OpenBadgeCredential.ValidUntil > checkDate;
        }
        else
        {
            verificationResult.CredentialIsNotExpired = null;
        }

        // verificationResult.CredentialIsNotExpired = true;


        // TODO Then check the revocation
        var checkRevocationStatusResult = await _mediator.Send(new CheckRevocationStatusRequest(request.OpenBadgeCredential), cancellationToken);
        if (checkRevocationStatusResult.IsFailed)
        {
            return checkRevocationStatusResult.ToResult();
        }

        verificationResult.CredentialIsNotRevoked = !checkRevocationStatusResult.Value.Revoked;
        verificationResult.ReferenceToRevocationList = checkRevocationStatusResult.Value.ReferenceToRevocationList;


        // TODO Then check the trust registry
        var checkTrustRegistryResult = await _mediator.Send(new CheckTrustRegistryRequest(request.OpenBadgeCredential), cancellationToken);
        if (checkTrustRegistryResult.IsFailed)
        {
            return checkTrustRegistryResult.ToResult();
        }

        verificationResult.IssuerFoundInTrustRegistry = checkTrustRegistryResult.Value.IssuerFoundInTrustRegistry;
        verificationResult.ReferenceToTrustRegistry = checkTrustRegistryResult.Value.ReferenceToTrustRegistry;

        return Result.Ok(verificationResult);
    }
}