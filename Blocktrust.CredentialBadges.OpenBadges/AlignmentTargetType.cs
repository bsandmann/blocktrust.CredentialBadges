namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlignmentTargetType
{
    [JsonPropertyName("ceasn:Competency")]
    CeasnCompetency,
    
    [JsonPropertyName("ceterms:Credential")]
    CetermsCredential,
    
    [JsonPropertyName("CFItem")]
    CFItem,
    
    [JsonPropertyName("CFRubric")]
    CFRubric,
    
    [JsonPropertyName("CFRubricCriterion")]
    CFRubricCriterion,
    
    [JsonPropertyName("CFRubricCriterionLevel")]
    CFRubricCriterionLevel,
    
    [JsonPropertyName("CTDL")]
    CTDL,
    
    // Additional proprietary terms must start with 'ext:'
}