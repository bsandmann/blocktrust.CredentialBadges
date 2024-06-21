using Blocktrust.CredentialBadges.Web.Domain;
using System.Text;

namespace Blocktrust.CredentialBadges.Web.Services.GenerateSnippetService;

public class SnippetsResult
{
    public string HtmlSnippet { get; set; }
    public string CssSnippet { get; set; }
    public string JsSnippet { get; set; }
}

public class GenerateSnippetService
{
    private readonly string _jsSnippetCdnUrl = "https://ourdomain.com/js/credential-status-update.js";

    public SnippetsResult GenerateSnippets(VerifiedCredential credential)
    {
        var htmlSnippet = GenerateHtmlSnippet(credential);
        var cssSnippet = GenerateCssSnippet();
        var jsSnippet = GenerateJavaScriptSnippet();

        return new SnippetsResult
        {
            HtmlSnippet = htmlSnippet,
            CssSnippet = cssSnippet,
            JsSnippet = jsSnippet
        };
    }

    private string GenerateHtmlSnippet(VerifiedCredential credential)
    {
        string statusColor = GetStatusColor(credential.Status);
        string statusIcon = GetStatusIcon(credential.Status);

        // Construct the URL or route based on the credential ID
        string url = $"/credentials/{credential.Id}";

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css\">");
        htmlBuilder.AppendLine($"<a id=\"credential-{credential.Id}\" href=\"{url}\" class=\"card\" data-credential-id=\"{credential.Id}\">");
        htmlBuilder.AppendLine($"  <div class=\"card-body\">");
        htmlBuilder.AppendLine($"    <h5 class=\"card-title\">{credential.Name}</h5>");
        htmlBuilder.AppendLine($"    <p class=\"card-text\">{credential.Description}</p>");
        htmlBuilder.AppendLine($"    <p id=\"credential-status-{credential.Id}\" class=\"{statusColor}\">Status: <i class=\"bi {statusIcon}\"></i> {credential.Status}</p>");
        htmlBuilder.AppendLine($"  </div>");
        htmlBuilder.AppendLine($"</a>");

        return htmlBuilder.ToString();
    }

    private string GenerateCssSnippet()
    {
        var cssBuilder = new StringBuilder();
        cssBuilder.AppendLine(".card {");
        cssBuilder.AppendLine("  text-decoration: none;"); // Ensure it looks like a card link
        cssBuilder.AppendLine("  display: block;"); // Ensure the whole card area is clickable
        cssBuilder.AppendLine("  border: 1px solid #ccc;");
        cssBuilder.AppendLine("  border-radius: 5px;");
        cssBuilder.AppendLine("  padding: 10px;");
        cssBuilder.AppendLine("  box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);");
        cssBuilder.AppendLine("  margin-bottom: 10px;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".card-title {");
        cssBuilder.AppendLine("  font-size: 1.2rem;");
        cssBuilder.AppendLine("  font-weight: bold;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".card-text {");
        cssBuilder.AppendLine("  margin-top: 10px;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-success {");
        cssBuilder.AppendLine("  color: green;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-danger {");
        cssBuilder.AppendLine("  color: red;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-warning {");
        cssBuilder.AppendLine("  color: orange;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-info {");
        cssBuilder.AppendLine("  color: blue;");
        cssBuilder.AppendLine("}");
        cssBuilder.AppendLine(".text-secondary {");
        cssBuilder.AppendLine("  color: gray;");
        cssBuilder.AppendLine("}");

        return cssBuilder.ToString();
    }

    private string GenerateJavaScriptSnippet()
    {
        return $"<script src=\"{_jsSnippetCdnUrl}\"></script>";
    }

    private string GetStatusColor(VerifiedCredential.CredentialStatus status)
    {
        return status switch
        {
            VerifiedCredential.CredentialStatus.Verified => "text-success",
            VerifiedCredential.CredentialStatus.Revoked => "text-danger",
            VerifiedCredential.CredentialStatus.Expired => "text-warning",
            VerifiedCredential.CredentialStatus.NotDue => "text-info",
            _ => "text-secondary",
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