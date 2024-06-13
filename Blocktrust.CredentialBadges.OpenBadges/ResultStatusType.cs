namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Defined vocabulary to convey the status of an achievement.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#resultstatustype-enumeration"/>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultStatusType
{
    /// <summary>
    /// The learner has successfully completed the achievement.
    /// This is the default status if no status result is included.
    /// </summary>
    [JsonPropertyName("Completed")]
    Completed,
    
    /// <summary>
    /// The learner is enrolled in the activity described by the achievement.
    /// </summary>
    [JsonPropertyName("Enrolled")]
    Enrolled,
   
    /// <summary>
    /// The learner has unsuccessfully completed the achievement.
    /// </summary>
    [JsonPropertyName("Failed")]
    Failed,
    
    /// <summary>
    /// The learner has started progress in the activity described by the achievement.
    /// </summary>
    [JsonPropertyName("InProgress")]
    InProgress,
   
    /// <summary>
    /// The learner has completed the activity described by the achievement, but
    /// successful completion has not been awarded, typically for administrative reasons.
    /// </summary>
    [JsonPropertyName("OnHold")]
    OnHold,
   
    /// <summary>
    /// The learner has completed the activity described by the achievement, but the
    /// completed result has not yet been confirmed.
    /// </summary>
    [JsonPropertyName("Provisional")]
    Provisional,
   
    /// <summary>
    /// The learner withdrew from the activity described by the achievement before completion.
    /// </summary>
    [JsonPropertyName("Withdrew")]
    Withdrew
}