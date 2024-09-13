using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blocktrust.CredentialBadges.OpenBadges;

/// <summary>
/// Represents a Status List 2021 Credential as defined in the W3C Verifiable Credentials Status List v2021 specification.
/// This credential encapsulates a status list for verifiable credentials.
/// <see href="https://www.w3.org/TR/2023/WD-vc-status-list-20230427/#statuslist2021credential"/>
/// </summary>
public class StatusList2021Credential
{
    /// <summary>
    /// The JSON-LD context of the credential.
    /// </summary>
    [JsonPropertyName("@context")]
    public string[] Context { get; set; }

    /// <summary>
    /// The type of the credential. MUST include "VerifiableCredential" and "StatusList2021Credential".
    /// </summary>
    [JsonPropertyName("type")]
    public string[] Type { get; set; }

    /// <summary>
    /// The issuer of the status list credential.
    /// </summary>
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }

    /// <summary>
    /// The unique identifier for the status list credential.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The issuance date of the status list credential.
    /// </summary>
    [JsonPropertyName("issuanceDate")]
    [JsonConverter(typeof(IssuanceDateConverter))]
    public DateTimeOffset IssuanceDate { get; set; }

    /// <summary>
    /// The subject of the status list credential, containing the actual status list.
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public StatusListCredentialSubject CredentialSubject { get; set; }

    /// <summary>
    /// The cryptographic proof that ensures the integrity of the credential.
    /// </summary>
    [JsonPropertyName("proof")]
    public Proof Proof { get; set; }
}

/// <summary>
/// Represents the subject of a Status List 2021 Credential, containing the actual status list.
/// </summary>
public class StatusListCredentialSubject
{
    /// <summary>
    /// The identifier for the credential subject.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The type of the credential subject. MUST be "StatusList2021".
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The purpose of the status list. MUST be either "revocation" or "suspension".
    /// </summary>
    [JsonPropertyName("statusPurpose")]
    public string StatusPurpose { get; set; }

    /// <summary>
    /// The GZIP-compressed, base64-encoded bitstring representing the status list.
    /// </summary>
    [JsonPropertyName("encodedList")]
    public string EncodedList { get; set; }
}

/// <summary>
/// Converts between JSON representations and DateTimeOffset for the issuance date.
/// Handles both Unix timestamp (long integer) and ISO 8601 date string formats.
/// </summary>
public class IssuanceDateConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// Reads the JSON representation and converts it to a DateTimeOffset.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader to read the JSON from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted DateTimeOffset value.</returns>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return DateTimeOffset.Parse(reader.GetString());
        }

        throw new JsonException("Unexpected token type for issuanceDate");
    }

    /// <summary>
    /// Writes a DateTimeOffset value as JSON.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("o"));
    }
}