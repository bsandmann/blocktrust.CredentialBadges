namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid.RegisterWallet;

using FluentResults;
using MediatR;

public class RegisterWalletRequest : IRequest<Result<Guid>>
{
    public string Seed { get; set; }
    public string Name { get; set; }
}