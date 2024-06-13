namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class Result
{
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    [JsonPropertyName("achievedLevel")]
    public Uri? AchievedLevel { get; set; }
    
    [JsonPropertyName("alignment")]
    public List<Alignment>? Alignment { get; set; }
    
    [JsonPropertyName("resultDescription")]
    public Uri? ResultDescription { get; set; }
    
    [JsonPropertyName("status")]
    public ResultStatusType? Status { get; set; }
    
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}