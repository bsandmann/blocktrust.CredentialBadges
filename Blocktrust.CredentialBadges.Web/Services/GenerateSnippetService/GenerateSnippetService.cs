using Blocktrust.CredentialBadges.Web.Domain;
using System.Text;

namespace Blocktrust.CredentialBadges.Web.Services.GenerateSnippetService;

public class SnippetsResult
{
    public string Snippet { get; set; }
}

public class GenerateSnippetService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GenerateSnippetService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public SnippetsResult GenerateSnippet(VerifiedCredential credential)
    {
        var snippet = GenerateHtmlSnippet(credential);
        return new SnippetsResult { Snippet = snippet };
    }

    private string GenerateHtmlSnippet(VerifiedCredential credential)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        string host = request != null ? $"{request.Scheme}://{request.Host}" : "https://credentialbadges.azurewebsites.net";
        string statusColor = GetStatusColor(credential.Status);
        string statusIcon = GetStatusIcon(credential.Status);

        // Construct the URL or route based on the credential ID
        string url = $"{host}/verifier/{credential.Id}";

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine($"<div class=\"credential-container\">");
        htmlBuilder.AppendLine($"  <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css\">");
        htmlBuilder.AppendLine($"  <a id=\"credential-{credential.Id}\" href=\"{url}\" class=\"credential-card\" data-credential-id=\"{credential.Id}\">");
        htmlBuilder.AppendLine($"    <div class=\"credential-card-body\">");
        htmlBuilder.AppendLine($"      <h5 class=\"credential-card-title\">{credential.Name}</h5>");
        htmlBuilder.AppendLine($"      <p class=\"credential-card-text\">{credential.Description}</p>");
        htmlBuilder.AppendLine($"      <p id=\"credential-status-{credential.Id}\" class=\"{statusColor}\">Status: <i class=\"bi {statusIcon}\"></i> {credential.Status}</p>");
        htmlBuilder.AppendLine($"    </div>");
        htmlBuilder.AppendLine($"  </a>");
        htmlBuilder.AppendLine($"  <script src=\"{host}/credential-status-update.js\"></script>");
        htmlBuilder.AppendLine($"</div>");
        return htmlBuilder.ToString();
    }

    private string GetStatusColor(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "text-success",
            VerifiedCredential.CredentialStatus.Revoked => "text-danger",
            VerifiedCredential.CredentialStatus.Expired => "text-warning",
            VerifiedCredential.CredentialStatus.NotDue => "text-primary",
            _ => "text-muted",
        };
    }

    private string GetStatusIcon(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "bi-check-circle-fill",
            VerifiedCredential.CredentialStatus.Revoked => "bi-x-circle-fill",
            VerifiedCredential.CredentialStatus.Expired => "bi-exclamation-circle-fill",
            VerifiedCredential.CredentialStatus.NotDue => "bi-clock-fill",
            _ => "bi-question-circle-fill",
        };
    }
}
