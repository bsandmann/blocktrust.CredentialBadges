namespace Blocktrust.CredentialBadges.Builder.Common;

public class AppSettings
{
    public string Agent1BaseUrl { get; set; }
    public string Agent2BaseUrl { get; set; }
    public string Agent1ApiKey { get; set; }
    public string Agent2ApiKey { get; set; }

    public string AdminAgentBaseUrl { get; set; }
    public string UserAgentBaseUrl { get; set; }
    
    public string AdminAgentAdminKey { get; set; }
    public string UserAgentAdminKey { get; set; }
    
    public string AdminApiKey { get; set; }
    
    
    public string IssuingDID { get; set; }
    
    public string SnippetBaseUrl { get; set; }
    
    /// <summary>
    /// API key for SendGrid
    /// </summary>
    public string? SendGridKey { get; set; }

    /// <summary>
    /// Configured Email for SendGrid
    /// </summary>
    public string? SendGridFromEmail { get; set; }

    
}