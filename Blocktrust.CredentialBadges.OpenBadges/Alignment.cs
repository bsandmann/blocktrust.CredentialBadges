namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

public class Alignment
{
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Alignment" };

    [JsonPropertyName("targetCode")]
    public string? TargetCode { get; set; }

    [JsonPropertyName("targetDescription")]
    public string? TargetDescription { get; set; }

    [JsonPropertyName("targetName")]
    public required string TargetName { get; set; }

    [JsonPropertyName("targetFramework")]
    public string? TargetFramework { get; set; }

    [JsonPropertyName("targetType")]
    public AlignmentTargetType? TargetType { get; set; }

    [JsonPropertyName("targetUrl")]
    public required Uri TargetUrl { get; set; }
}
