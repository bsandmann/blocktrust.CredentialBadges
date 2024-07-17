using MediatR;
using FluentResults;

namespace Blocktrust.CredentialBadges.Builder.Commands.NewDid;

public class RegisterEntityRequest : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public Guid WalletId { get; set; }
}