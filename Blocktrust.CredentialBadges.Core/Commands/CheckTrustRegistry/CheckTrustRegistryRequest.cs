namespace Blocktrust.CredentialBadges.Core.Commands.CheckTrustRegistry;

using FluentResults;
using MediatR;
using OpenBadges;

public class CheckTrustRegistryRequest : IRequest<Result<CheckTrustRegistryResponse>>
{
    public CheckTrustRegistryRequest(OpenBadgeCredential openBadgeCredential)
    {
        OpenBadgeCredential = openBadgeCredential;
    }

    public OpenBadgeCredential OpenBadgeCredential { get; }
}