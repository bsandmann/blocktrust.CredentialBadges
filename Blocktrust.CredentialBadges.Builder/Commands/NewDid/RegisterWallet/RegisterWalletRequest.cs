using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterWalletRequest : IRequest<Result<Guid>>
{
    public string Seed { get; set; }
    public string Name { get; set; }
}