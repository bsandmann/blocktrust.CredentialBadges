namespace Blocktrust.CredentialBadges.OpenBadges;

using System;
using System.Text.Json.Serialization;

/// <summary>
/// A JSON-LD Linked Data proof.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#proof"/>
/// </summary>
public class Proof
{
    /// <summary>
    /// Signature suite used to produce proof. [1]
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    /// <summary>
    /// Date the proof was created. [0..1]
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    /// <summary>
    /// The suite used to create the proof. [0..1]
    /// </summary>
    [JsonPropertyName("cryptosuite")]
    public string? Cryptosuite { get; set; }

    /// <summary>
    /// A value chosen by the verifier to mitigate authentication proof replay attacks. [0..1]
    /// </summary>
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }

    /// <summary>
    /// The domain of the proof to restrict its use to a particular target. [0..1]
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// A value chosen by the creator of proof to randomize proof values for privacy purposes. [0..1]
    /// </summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    /// <summary>
    /// The purpose of the proof to be used with verificationMethod. MUST be 'assertionMethod'. [0..1]
    /// </summary>
    [JsonPropertyName("proofPurpose")]
    public string? ProofPurpose { get; set; }

    /// <summary>
    /// Value of the proof. [0..1]
    /// </summary>
    [JsonPropertyName("proofValue")]
    public string? ProofValue { get; set; }

    /// <summary>
    /// The URL of the public key that can verify the signature. [0..1]
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    public Uri? VerificationMethod { get; set; }
}