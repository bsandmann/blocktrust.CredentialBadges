namespace Blocktrust.CredentialBadges.Core.Commands.CheckTrustRegistry;

using FluentResults;
using MediatR;

public class CheckTrustRegistryHandler : IRequestHandler<CheckTrustRegistryRequest,Result<CheckTrustRegistryResponse>>
{
    public async Task<Result<CheckTrustRegistryResponse>> Handle(CheckTrustRegistryRequest request, CancellationToken cancellationToken)
    {
        // inject some configutation of configured trustregistries
        
        // inject HttpClient and do the checks
        
        return  Result.Ok(new CheckTrustRegistryResponse()
        {
            IssuerFoundInTrustRegistry = true,
            ReferenceToTrustRegistry = "https://???"
        });
    }
}