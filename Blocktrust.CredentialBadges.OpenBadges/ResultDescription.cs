namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;
using Enums;

/// <summary>
/// Describes a possible achievement result.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#resultdescription"/>
/// </summary>
public class ResultDescription
{
    /// <summary>
    /// The unique URI for this result description. Required so a result can link to this result description. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the IRI 'ResultDescription'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "ResultDescription" };

    /// <summary>
    /// Alignments between this result description and nodes in external frameworks. [0..*]
    /// </summary>
    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }

    /// <summary>
    /// An ordered list of allowed values. The values should be ordered from low to high as determined by the achievement creator. [0..*]
    /// </summary>
    [JsonPropertyName("allowedValue")]
    public List<string>? AllowedValue { get; set; }

    /// <summary>
    /// The name of the result. [1]
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The id of the rubric criterion level required to pass as determined by the achievement creator. [0..1]
    /// </summary>
    [JsonPropertyName("requiredLevel")]
    public Uri? RequiredLevel { get; set; }

    /// <summary>
    /// A value from allowedValue or within the range of valueMin to valueMax required to pass as determined by the achievement creator. [0..1]
    /// </summary>
    [JsonPropertyName("requiredValue")]
    public string? RequiredValue { get; set; }

    /// <summary>
    /// The type of result this description represents. This is an extensible enumerated vocabulary. [1]
    /// </summary>
    [JsonPropertyName("resultType")]
    public required EResultType Enumeration { get; set; }

    /// <summary>
    /// An ordered array of rubric criterion levels that may be asserted in the linked result. The levels should be ordered from low to high as determined by the achievement creator. [0..*]
    /// </summary>
    [JsonPropertyName("rubricCriterionLevel")]
    public List<RubricCriterionLevel>? RubricCriterionLevel { get; set; }

    /// <summary>
    /// The maximum possible value that may be asserted in a linked result. [0..1]
    /// </summary>
    [JsonPropertyName("valueMax")]
    public string? ValueMax { get; set; }

    /// <summary>
    /// The minimum possible value that may be asserted in a linked result. [0..1]
    /// </summary>
    [JsonPropertyName("valueMin")]
    public string? ValueMin { get; set; }
}
