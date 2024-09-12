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
    public long IssuanceDate { get; set; }

    /// <summary>
    /// The subject of the status list credential, containing the actual status list.
    /// </summary>
    [JsonPropertyName("credentialSubject")]
    public CredentialSubject CredentialSubject { get; set; }

    /// <summary>
    /// The cryptographic proof that ensures the integrity of the credential.
    /// </summary>
    [JsonPropertyName("proof")]
    public Proof Proof { get; set; }
}

/// <summary>
/// Represents the subject of a Status List 2021 Credential, containing the actual status list.
/// </summary>
public class CredentialSubject
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