using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.DIDKey.DIDKeySignatureVerification;
using FluentResults;
using MediatR;

namespace Blocktrust.CredentialBadges.Core.Commands.CheckDIDKeySignature;

public class CheckDIDKeySignatureHandler : IRequestHandler<CheckDIDKeySignatureRequest, Result<ECheckSignatureResponse>>
{
    private readonly ISha256Service _sha256Service;
    private readonly IEcService _ecService;

    public CheckDIDKeySignatureHandler(ISha256Service sha256Service, IEcService ecService)
    {
        _sha256Service = sha256Service;
        _ecService = ecService;
    }

    public Task<Result<ECheckSignatureResponse>> Handle(CheckDIDKeySignatureRequest request, CancellationToken cancellationToken)
    {
        
        var verifier = new DIDKeySignatureVerification(_sha256Service, _ecService);
        var result = verifier.VerifySignature(request.CredentialJson);
        return Task.FromResult(result);
    }
}