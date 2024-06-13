namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class AchievementSubject
{
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }
    
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; }

    [JsonPropertyName("activityEndDate")]
    public DateTime? ActivityEndDate { get; set; }
    
    [JsonPropertyName("activityStartDate")]
    public DateTime? ActivityStartDate { get; set; }
    
    [JsonPropertyName("creditsEarned")]
    public decimal? CreditsEarned { get; set; }
    
    [JsonPropertyName("achievement")]
    public required Achievement Achievement { get; set; }
    
    [JsonPropertyName("identifier")]
    public List<string>? Identifier { get; set; }
    
    [JsonPropertyName("image")]
    public Image? Image { get; set; }
    
    [JsonPropertyName("licenseNumber")]
    public string? LicenseNumber { get; set; } 
    
    [JsonPropertyName("narrative")]
    public string? Narrative { get; set; } 
    
    [JsonPropertyName("result")]
    public List<Result>? Result { get; set; } 
    
    [JsonPropertyName("role")]
    public string? Role { get; set; } 
    
    [JsonPropertyName("source")]
    public Profile? Source { get; set; } 
    
    [JsonPropertyName("term")]
    public string? Term { get; set; } 
}