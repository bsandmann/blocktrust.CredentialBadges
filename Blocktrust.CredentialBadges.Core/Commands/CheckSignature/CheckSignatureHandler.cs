namespace Blocktrust.CredentialBadges.Core.Commands.CheckSignature;

using FluentResults;
using MediatR;

public class CheckSignatureHandler : IRequestHandler<CheckSignatureRequest,Result<ECheckSignatureResponse>>
{
    public async Task<Result<ECheckSignatureResponse>> Handle(CheckSignatureRequest request, CancellationToken cancellationToken)
    {
        // inject some configutation of supported did methods
        
        // dependingon the DID method, we might need to conntact a resolver to get the DID Document for the public key
        
        return  Result.Ok(ECheckSignatureResponse.Valid);
    }
}