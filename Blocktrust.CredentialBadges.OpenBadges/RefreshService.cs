namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// The information in RefreshService is used to refresh the verifiable credential.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#refreshservice"/>
/// </summary>
public class RefreshService
{
    /// <summary>
    /// The value MUST be the URL of the issuer's refresh service. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The name of the refresh service method. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}