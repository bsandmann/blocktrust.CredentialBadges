namespace Blocktrust.CredentialBadges.Builder.Common;

using System;

public class AppSettings
{
    // Main Agent URLs
    public string AdminAgentBaseUrl { get; set; }
    public string UserAgentBaseUrl { get; set; }
    
    // Agent API Keys (used for both admin operations and regular API calls)
    public string AdminApiKey { get; set; }
    public string UserApiKey { get; set; }
    
    // DID Information
    public string IssuingDID { get; set; }
    
    // Optional Subject Info
    public string? SubjectDID { get; set; }
    
    // Deprecated properties - use the new ones above instead
    [Obsolete("Use UserApiKey instead")]
    public string SubjectApiKey { get; set; }
    
    [Obsolete("Use AdminApiKey instead")]
    public string AdminAgentAdminKey { get; set; }
    
    [Obsolete("Use UserApiKey instead")]
    public string UserAgentAdminKey { get; set; }
    
    // Integration URLs
    public string SnippetsUrl { get; set; }
    
    /// <summary>
    /// API key for SendGrid
    /// </summary>
    public string? SendGridKey { get; set; }

    /// <summary>
    /// Configured Email for SendGrid
    /// </summary>
    public string? SendGridFromEmail { get; set; }
}