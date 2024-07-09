namespace Blocktrust.CredentialBadges.Web.Commands.VerifiedCredentials.UpdateTemplateId;

using MediatR;
using FluentResults;

public class UpdateCredentialTemplateIdRequest : IRequest<Result>
{
    public Guid CredentialId { get; set; }
    public string TemplateId { get; set; }

    public UpdateCredentialTemplateIdRequest(Guid credentialId, string templateId)
    {
        CredentialId = credentialId;
        TemplateId = templateId;
    }
}