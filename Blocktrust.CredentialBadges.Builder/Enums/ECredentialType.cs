namespace Blocktrust.CredentialBadges.Builder.Enums;

using System.Text.Json.Serialization;

/// <summary>
/// Enumeration for credential types (Achievement or Endorsement)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ECredentialType
{
    /// <summary>
    /// Represents an achievement credential
    /// </summary>
    [JsonPropertyName("Achievement")]
    Achievement,
    
    /// <summary>
    /// Represents an endorsement credential
    /// </summary>
    [JsonPropertyName("Endorsement")]
    Endorsement
}