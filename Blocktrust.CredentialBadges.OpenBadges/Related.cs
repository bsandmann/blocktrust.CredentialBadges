namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Identifies a related achievement.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#related"/>
/// </summary>
public class Related
{
    /// <summary>
    /// The related achievement. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value of the type property MUST be an unordered set.
    /// One of the items MUST be the IRI 'Related'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Related" };

    /// <summary>
    /// The language of the related achievement. [0..1]
    /// </summary>
    [JsonPropertyName("inLanguage")]
    public string? InLanguage { get; set; }

    /// <summary>
    /// The version of the related achievement. [0..1]
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}