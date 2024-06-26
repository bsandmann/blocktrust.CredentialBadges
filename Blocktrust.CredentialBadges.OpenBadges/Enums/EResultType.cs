namespace Blocktrust.CredentialBadges.OpenBadges.Enums;

using System.Text.Json.Serialization;

/// <summary>
/// The type of result. This is an extensible enumerated vocabulary.
/// Extending the vocabulary makes use of a naming convention.
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#resulttype-enumeration"/>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EResultType
{
    /// <summary>
    /// The result is a grade point average.
    /// </summary>
    GradePointAverage,

    /// <summary>
    /// The result is a letter grade.
    /// </summary>
    LetterGrade,

    /// <summary>
    /// The result is a percent score.
    /// </summary>
    Percent,

    /// <summary>
    /// The result is a performance level.
    /// </summary>
    PerformanceLevel,

    /// <summary>
    /// The result is a predicted score.
    /// </summary>
    PredictedScore,

    /// <summary>
    /// The result is a raw score.
    /// </summary>
    RawScore,

    /// <summary>
    /// A generic result.
    /// </summary>
    Result,

    /// <summary>
    /// The result is from a rubric criterion.
    /// </summary>
    RubricCriterion,

    /// <summary>
    /// The result is a rubric criterion level.
    /// </summary>
    RubricCriterionLevel,

    /// <summary>
    /// The result represents a rubric score with both a name and a numeric value.
    /// </summary>
    RubricScore,

    /// <summary>
    /// The result is a scaled score.
    /// </summary>
    ScaledScore,

    /// <summary>
    /// The result conveys the status of the achievement.
    /// </summary>
    Status
}