namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Describes an alignment between an achievement and a node in
/// an educational framework.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#alignment"/>
/// </summary>
public class Alignment
{
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the
    /// items MUST be the IRI 'Alignment'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Alignment" };

    /// <summary>
    /// If applicable, a locally unique string identifier that identifies the
    /// alignment target within its framework and/or targetUrl. [0..1]
    /// </summary>
    [JsonPropertyName("targetCode")]
    public string? TargetCode { get; set; }

    /// <summary>
    /// Short description of the alignment target. [0..1]
    /// </summary>
    [JsonPropertyName("targetDescription")]
    public string? TargetDescription { get; set; }

    /// <summary>
    /// Name of the alignment. [1]
    /// </summary>
    [JsonPropertyName("targetName")]
    public required string TargetName { get; set; }

    /// <summary>
    /// Name of the framework the alignment target. [0..1]
    /// </summary>
    [JsonPropertyName("targetFramework")]
    public string? TargetFramework { get; set; }

    /// <summary>
    /// The type of the alignment target node. [0..1]
    /// </summary>
    [JsonPropertyName("targetType")]
    public EAlignmentTargetType? TargetType { get; set; }

    /// <summary>
    /// URL linking to the official description of the alignment target, for
    /// example an individual standard within an educational framework. [1]
    /// </summary>
    [JsonPropertyName("targetUrl")]
    public required Uri TargetUrl { get; set; }
}
