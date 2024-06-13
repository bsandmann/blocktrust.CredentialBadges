namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// The type of the alignment target node in the target framework.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#alignmenttargettype-enumeration"/>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlignmentTargetType
{
    /// <summary>
    /// An alignment to a CTDL-ASN/CTDL competency published by Credential Engine.
    /// </summary>
    [JsonPropertyName("ceasn:Competency")]
    CeasnCompetency,
   
    /// <summary>
    /// An alignment to a CTDL Credential published by Credential Engine.
    /// </summary>
    [JsonPropertyName("ceterms:Credential")]
    CetermsCredential,
   
    /// <summary>
    /// An alignment to a CASE Framework Item.
    /// </summary>
    [JsonPropertyName("CFItem")]
    CFItem,
   
    /// <summary>
    /// An alignment to a CASE Framework Rubric.
    /// </summary>
    [JsonPropertyName("CFRubric")]
    CFRubric,
    
    /// <summary>
    /// An alignment to a CASE Framework Rubric Criterion.
    /// </summary>
    [JsonPropertyName("CFRubricCriterion")]
    CFRubricCriterion,
   
    /// <summary>
    /// An alignment to a CASE Framework Rubric Criterion Level.
    /// </summary>
    [JsonPropertyName("CFRubricCriterionLevel")]
    CFRubricCriterionLevel,
    
    /// <summary>
    /// An alignment to a Credential Engine Item.
    /// </summary>
    [JsonPropertyName("CTDL")]
    CTDL,
    
    // Additional proprietary terms must start with 'ext:'
}