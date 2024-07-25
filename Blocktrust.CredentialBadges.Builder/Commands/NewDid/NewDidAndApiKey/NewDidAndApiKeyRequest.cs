namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.NewDidAndApiKey;

using FluentResults;
using MediatR;

public class NewDidAndApiKeyRequest : IRequest<Result<DidAndApiKeyResponse>>
{
    public string Seed { get; set; }
    public string WalletName { get; set; }
}