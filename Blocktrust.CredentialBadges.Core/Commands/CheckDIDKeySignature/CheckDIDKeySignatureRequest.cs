using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckDIDKeySignature;

public class CheckDIDKeySignatureRequest : IRequest<Result<ECheckSignatureResponse>>
{
    public string CredentialJson { get; }

    public CheckDIDKeySignatureRequest(string credentialJson)
    {
        CredentialJson = credentialJson;
    }
}