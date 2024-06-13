namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultStatusType
{
    [JsonPropertyName("Completed")]
    Completed,
    
    [JsonPropertyName("Enrolled")]
    Enrolled,
    
    [JsonPropertyName("Failed")]
    Failed,
    
    [JsonPropertyName("InProgress")]
    InProgress,
    
    [JsonPropertyName("OnHold")]
    OnHold,
    
    [JsonPropertyName("Provisional")]
    Provisional,
    
    [JsonPropertyName("Withdrew")]
    Withdrew
}