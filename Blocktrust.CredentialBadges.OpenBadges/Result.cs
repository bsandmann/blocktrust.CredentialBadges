namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;
using Enums;

/// <summary>
/// Describes a result that was achieved.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#result"/>
/// </summary>
public class Result
{
    /// <summary>
    /// The value of the type property MUST be an unordered set.
    /// One of the items MUST be the IRI 'Result'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    /// <summary>
    /// If the result represents an achieved rubric criterion level
    /// (e.g. Mastered), the value is the id of the RubricCriterionLevel
    /// in linked ResultDescription. [0..1]
    /// </summary>
    [JsonPropertyName("achievedLevel")]
    public Uri? AchievedLevel { get; set; }
    
    /// <summary>
    /// The alignments between this result and nodes in external frameworks.
    /// This set of alignments are in addition to the set of alignments
    /// defined in the corresponding ResultDescription object. [0..*]
    /// </summary>
    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }
    
    /// <summary>
    /// An achievement can have many result descriptions describing possible
    /// results. The value of resultDescription is the id of the result description
    /// linked to this result. The linked result description must be in the
    /// achievement that is being asserted. [0..1]
    /// </summary>
    [JsonPropertyName("resultDescription")]
    public Uri? ResultDescription { get; set; }
    
    /// <summary>
    /// The status of the achievement. Required if resultType of the linked
    /// ResultDescription is Status. [0..1]
    /// </summary>
    [JsonPropertyName("status")]
    public EResultStatusType? Status { get; set; }
    
    /// <summary>
    /// A string representing the result of the performance, or demonstration, of
    /// the achievement. For example, 'A' if the recipient received an A grade in class. [0..1]
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}