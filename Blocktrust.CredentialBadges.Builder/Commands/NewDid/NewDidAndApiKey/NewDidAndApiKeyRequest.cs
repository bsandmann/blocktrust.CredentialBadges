using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class NewDidAndApiKeyRequest : IRequest<Result>
{
    public string Seed { get; set; }
    public string WalletName { get; set; }
}