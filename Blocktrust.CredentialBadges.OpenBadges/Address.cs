namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// An address for the described entity.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#address"/>
/// </summary>
public class Address
{
    /// <summary>
    /// The value of the type property MUST be an unordered set. One of the items MUST be the IRI 'Address'. [1..*]
    /// </summary>
    [JsonPropertyName("type")]
    public required List<string> Type { get; set; } = new List<string> { "Address" };

    /// <summary>
    /// A country. [0..1]
    /// </summary>
    [JsonPropertyName("addressCountry")]
    public string? AddressCountry { get; set; }

    /// <summary>
    /// A country code. The value must be a ISO 3166-1 alpha-2 country code [ISO3166-1]. [0..1]
    /// </summary>
    [JsonPropertyName("addressCountryCode")]
    public string? AddressCountryCode { get; set; }

    /// <summary>
    /// A region within the country. [0..1]
    /// </summary>
    [JsonPropertyName("addressRegion")]
    public string? AddressRegion { get; set; }

    /// <summary>
    /// A locality within the region. [0..1]
    /// </summary>
    [JsonPropertyName("addressLocality")]
    public string? AddressLocality { get; set; }

    /// <summary>
    /// A street address within the locality. [0..1]
    /// </summary>
    [JsonPropertyName("streetAddress")]
    public string? StreetAddress { get; set; }

    /// <summary>
    /// A post office box number for PO box addresses. [0..1]
    /// </summary>
    [JsonPropertyName("postOfficeBoxNumber")]
    public string? PostOfficeBoxNumber { get; set; }

    /// <summary>
    /// A postal code. [0..1]
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// The geographic coordinates of the location. [0..1]
    /// </summary>
    [JsonPropertyName("geo")]
    public GeoCoordinates? Geo { get; set; }
}

