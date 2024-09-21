namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Metadata about images that represent assertions, achieve or profiles.
/// These properties can typically be represented as just the id string of the image,
/// but using a fleshed-out document allows for including captions and other applicable metadata.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#image"/>
/// </summary>
public class Image
{
    /// <summary>
    /// The URI or Data URI of the image. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// MUST be the IRI 'Image'. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; } = "Image";

    /// <summary>
    /// The caption for the image. [0..1]
    /// </summary>
    [JsonPropertyName("caption")]
    public string? Caption { get; set; }
}