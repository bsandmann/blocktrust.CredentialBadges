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
    public SnippetsResult GenerateSnippets(VerifiedCredential credential)
    {
        var htmlSnippet = GenerateHtmlSnippet(credential);
        var cssSnippet = GenerateCssSnippet();
        var jsSnippet = GenerateJavaScriptSnippet(credential);

        return new SnippetsResult
        {
            HtmlSnippet = htmlSnippet,
            CssSnippet = cssSnippet,
            JsSnippet = jsSnippet
        };
    }

    private string GenerateHtmlSnippet(VerifiedCredential credential)
    {
        string statusColor = credential.Status == VerifiedCredential.CredentialStatus.Verified ? "text-success" : "text-danger";
        string statusIcon = credential.Status == VerifiedCredential.CredentialStatus.Verified ? "bi-check-circle-fill" : "bi-x-circle-fill";

        // Construct the URL or route based on the credential ID
        string url = $"/credentials/{credential.Id}";

        var htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine($"<a href=\"{url}\" class=\"card\">");
        htmlBuilder.AppendLine($"  <div class=\"card-body\">");
        htmlBuilder.AppendLine($"    <h5 class=\"card-title\">{credential.Name}</h5>");
        htmlBuilder.AppendLine($"    <p class=\"card-text\">{credential.Description}</p>");
        htmlBuilder.AppendLine($"    <p class=\"{statusColor}\">Status: <i class=\"bi {statusIcon}\"></i> {credential.Status}</p>");
        htmlBuilder.AppendLine($"  </div>");
        htmlBuilder.AppendLine($"</a>");

        return htmlBuilder.ToString();
    }

    private string GenerateCssSnippet()
    {
        var cssBuilder = new StringBuilder();
        cssBuilder.AppendLine(".card {");
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

        return cssBuilder.ToString();
    }

    private string GenerateJavaScriptSnippet(VerifiedCredential credential)
    {
        var jsBuilder = new StringBuilder();
        jsBuilder.AppendLine("const credentialId = '{credential.Id}';");
        jsBuilder.AppendLine("// Function to update status via API");
        jsBuilder.AppendLine("function updateStatus() {");
        jsBuilder.AppendLine("  // Make API request to update status");
        jsBuilder.AppendLine("}");

        return jsBuilder.ToString();
    }
}