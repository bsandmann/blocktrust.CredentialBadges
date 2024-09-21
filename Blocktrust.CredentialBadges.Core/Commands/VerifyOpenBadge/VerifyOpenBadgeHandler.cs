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
        //current date
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
        
        if (request.OpenBadgeCredential.ValidFrom is not null)
        {
            verificationResult.CredentialIssuanceDateIsNotInFuture = request.OpenBadgeCredential.ValidFrom < checkDate;
        }
        else
        {
            verificationResult.CredentialIssuanceDateIsNotInFuture = true;
        }

        if (request.OpenBadgeCredential.ValidUntil is not null)
        {
            verificationResult.CredentialIsNotExpired = request.OpenBadgeCredential.ValidUntil > checkDate;
        }
        else
        {
            verificationResult.CredentialIsNotExpired = null;
        }



        // check the revocation
            //if CredentialStatus is not null
            if (request.OpenBadgeCredential.CredentialStatus is not null)
            {
                var checkRevocationStatusResult = await _mediator.Send(new CheckRevocationStatusRequest(request.OpenBadgeCredential.CredentialStatus), cancellationToken);

                verificationResult.CredentialIsNotRevoked = !checkRevocationStatusResult.IsRevoked;
                verificationResult.ReferenceToRevocationList = checkRevocationStatusResult.CredentialId; 
            }
            else 
            {
                verificationResult.CredentialIsNotRevoked = true;
                verificationResult.ReferenceToRevocationList = "";
            }
        
  


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