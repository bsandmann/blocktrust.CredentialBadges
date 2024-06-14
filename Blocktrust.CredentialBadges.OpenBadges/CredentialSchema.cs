namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// Identify the type and location of a data schema.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#credentialschema"/>
/// </summary>
public class CredentialSchema
{
    /// <summary>
    /// The value MUST be a URI identifying the schema file. One instance of CredentialSchema 
    /// MUST have an id that is the URL of the JSON Schema for this credential defined by this specification. [1]
    /// </summary>
    [JsonPropertyName("id")]
    public required Uri Id { get; set; }

    /// <summary>
    /// The value MUST identify the type of data schema validation. One instance of CredentialSchema 
    /// MUST have a type of 'JsonSchemaValidator2019'. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; } = "JsonSchemaValidator2019";
}