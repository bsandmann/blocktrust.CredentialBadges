namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// The geographic coordinates of a location.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#geocoordinates"/>
/// </summary>
public class GeoCoordinates
{
    /// <summary>
    /// The value of the type property MUST be an unordered set.
    /// One of the items MUST be the IRI 'GeoCoordinates'. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; } = "GeoCoordinates";

    /// <summary>
    /// The latitude of the location [WGS84]. [1]
    /// </summary>
    [JsonPropertyName("latitude")]
    public required double Latitude { get; set; }

    /// <summary>
    /// The longitude of the location [WGS84]. [1]
    /// </summary>
    [JsonPropertyName("longitude")]
    public required double Longitude { get; set; }
}