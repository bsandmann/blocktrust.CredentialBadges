namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class Image
{
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }
    
    [JsonPropertyName("type")]
    public required string Type { get; set; } = "Image";

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }
}