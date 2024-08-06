namespace Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

using FluentResults;
using MediatR;

public class CheckSignatureHandler : IRequestHandler<CheckSignatureRequest,Result<ECheckSignatureResponse>>
{
    public async Task<Result<ECheckSignatureResponse>> Handle(CheckSignatureRequest request, CancellationToken cancellationToken)
    {
        // inject some configutation of supported did methods
        
        // dependingon the DID method, we might need to conntact a resolver to get the DID Document for the public key
       
        // DID-PRISM -> the public key is in the issuer itself (must be decoded by bjoern)
        
        // DID-KEY -> the public key is in the issuer itself
        // DID-WEB -> either use the universal resolver or query the website ourselves
        
        return  Result.Ok(ECheckSignatureResponse.Valid);
    }
}