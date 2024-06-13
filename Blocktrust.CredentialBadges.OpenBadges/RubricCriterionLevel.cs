namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Describes a rubric criterion level.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#rubriccriterionlevel"/>
/// </summary>
public class RubricCriterionLevel
{
    /// <summary>
    /// The unique URI for this rubric criterion level. Required so a result can link to this rubric criterion level. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the IRI 'RubricCriterionLevel'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "RubricCriterionLevel" };

    /// <summary>
    /// Alignments between this rubric criterion level and a rubric criterion levels defined in external frameworks. [0..*]
    /// </summary>
    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }

    /// <summary>
    /// Description of the rubric criterion level. [0..1]
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The rubric performance level in terms of success. [0..1]
    /// </summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>
    /// The name of the rubric criterion level. [1]
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The points associated with this rubric criterion level. [0..1]
    /// </summary>
    [JsonPropertyName("points")]
    public string? Points { get; set; }
}