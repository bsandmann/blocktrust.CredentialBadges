namespace Blocktrust.CredentialBadges.OpenBadges;

using System.Text.Json.Serialization;

/// <summary>
/// Enumeration for AchievementType
/// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#achievementtype-enumeration"/>
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AchievementType
{
    /// <summary>
    /// Represents a generic achievement.
    /// </summary>
    [JsonPropertyName("Achievement")]
    Achievement,

    /// <summary>
    /// Credential earned through work-based learning and earn-and-learn models
    /// that meet standards and are applicable to industry trades and professions.
    /// This is an exact match of ApprenticeshipCertificate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("ApprenticeshipCertificate")]
    ApprenticeshipCertificate,

    /// <summary>
    /// Direct, indirect, formative, and summative evaluation or estimation of the
    /// nature, ability, or quality of an entity, performance, or outcome of an action.
    /// This is an exact match of Assessment in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Assessment")]
    Assessment,

    /// <summary>
    /// Represents the result of a curricular, or co-curricular assignment or exam.
    /// </summary>
    [JsonPropertyName("Assignment")]
    Assignment,

    /// <summary>
    /// College/university award for students typically completing the first one
    /// to two years of post secondary school education.
    /// Equivalent to an award at UNESCO ISCED 2011, Level 5.
    /// This is an exact match of AssociateDegree in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("AssociateDegree")]
    AssociateDegree,

    /// <summary>
    /// Represents an award.
    /// </summary>
    [JsonPropertyName("Award")]
    Award,

    /// <summary>
    /// Visual symbol containing verifiable claims in accordance with the
    /// Open Badges specification and delivered digitally.
    /// This is an exact match of Badge in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Badge")]
    Badge,

    /// <summary>
    /// College/university award for students typically completing three to
    /// five years of education where course work and activities advance skills
    /// beyond those of the first one to two years of college/university study.
    /// Equivalent to an award at UNESCO ISCED 2011, Level 6.
    /// Use for 5-year cooperative (work-study) programs. A cooperative plan
    /// provides for alternate class attendance and employment in business, industry,
    /// or government; thus, it allows students to combine actual work experience
    /// with their college studies.
    /// Also includes bachelor's degrees in which the normal 4 years of work are
    /// completed in 3 years.
    /// This is an exact match of BachelorDegree in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("BachelorDegree")]
    BachelorDegree,

    /// <summary>
    /// Credential that designates requisite knowledge and skills of an occupation,
    /// profession, or academic program.
    /// This is an exact match of Certificate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Certificate")]
    Certificate,

    /// <summary>
    /// Credential that acknowledges completion of an assignment, training
    /// or other activity.
    /// A record of the activity may or may not exist, and the credential
    /// may or may not be designed as preparation for another resource such
    /// as a credential, assessment, or learning opportunity.
    /// This is an exact match of CertificateOfCompletion in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("CertificateOfCompletion")]
    CertificateOfCompletion,

    /// <summary>
    /// Time-limited, revocable, renewable credential awarded by an authoritative
    /// body for demonstrating the knowledge, skills, and abilities to perform
    /// specific tasks or an occupation.
    /// Certifications can typically be revoked if not renewed, for a violation
    /// of a code of ethics (if applicable) or proven incompetence after due process.
    /// Description of revocation criteria for a specific Certification should
    /// be defined using Revocation Profile.
    /// This is an exact match of Certification in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Certification")]
    Certification,

    /// <summary>
    /// Represents community service.
    /// </summary>
    [JsonPropertyName("CommunityService")]
    CommunityService,

    /// <summary>
    /// Measurable or observable knowledge, skill, or ability necessary to
    /// successful performance of a person.
    /// This is an exact match of Competency in [CTDL-ASN-TERMS].
    /// </summary>
    [JsonPropertyName("Competency")]
    Competency,

    /// <summary>
    /// Represents a course completion.
    /// </summary>
    [JsonPropertyName("Course")]
    Course,

    /// <summary>
    /// Represents a co-curricular activity.
    /// </summary>
    [JsonPropertyName("CoCurricular")]
    CoCurricular,

    /// <summary>
    /// Academic credential conferred upon completion of a program or course of
    /// study, typically over multiple years at a college or university.
    /// This is an exact match of Degree in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Degree")]
    Degree,

    /// <summary>
    /// Credential awarded by educational institutions for successful completion
    /// of a course of study or its equivalent.
    /// This is an exact match of Diploma in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("Diploma")]
    Diploma,

    /// <summary>
    /// Highest credential award for students who have completed both a bachelor's
    /// degree and a master's degree or their equivalent as well as independent
    /// research and/or a significant project or paper.
    /// Equivalent to UNESCO ISCED, Level 8.
    /// This is an exact match of DoctoralDegree in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("DoctoralDegree")]
    DoctoralDegree,

    /// <summary>
    /// Represents practical activities that are done away school, college, or
    /// place of work. Includes internships and practicums.
    /// </summary>
    [JsonPropertyName("Fieldwork")]
    Fieldwork,

    /// <summary>
    /// (GED) Credential awarded by examination that demonstrates that an individual
    /// has acquired secondary school-level academic skills.
    /// Equivalent to a secondary school diploma, based on passing a state- or
    /// province-selected examination such as GED, HiSET, or TASC; or to an
    /// award at UNESCO ISCED 2011 Levels 2 or 3.
    /// This is an exact match of GeneralEducationDevelopment in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("GeneralEducationDevelopment")]
    GeneralEducationDevelopment,

    /// <summary>
    /// Credential awarded to skilled workers on successful completion of an
    /// apprenticeship in industry trades and professions.
    /// This is an exact match of JourneymanCertificate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("JourneymanCertificate")]
    JourneymanCertificate,

    /// <summary>
    /// Set of learning opportunities that leads to an outcome, usually a
    /// credential like a degree or certificate.
    /// This is an exact match of LearningProgram in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("LearningProgram")]
    LearningProgram,

    /// <summary>
    /// Credential awarded by a government agency or other authorized organization
    /// that constitutes legal authority to do a specific job and/or utilize a specific
    /// item, system or infrastructure and are typically earned through some combination
    /// of degree or certificate attainment, certifications, assessments, work experience,
    /// and/or fees, and are time-limited and must be renewed periodically.
    /// This is an exact match of License in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("License")]
    License,

    /// <summary>
    /// Represents membership.
    /// </summary>
    [JsonPropertyName("Membership")]
    Membership,

    /// <summary>
    /// Doctoral degree conferred upon completion of a program providing the knowledge
    /// and skills for the recognition, credential, or license required for professional practice.
    /// Equivalent to an award at UNESCO ISCED 2011, Level 8.
    /// This is an exact match of ProfessionalDoctorate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("ProfessionalDoctorate")]
    ProfessionalDoctorate,

    /// <summary>
    /// Credential assuring that an organization, program, or awarded credential meets
    /// prescribed requirements and may include development and administration of
    /// qualifying examinations.
    /// This is an exact match of QualityAssuranceCredential in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("QualityAssuranceCredential")]
    QualityAssuranceCredential,

    /// <summary>
    /// Credential awarded upon demonstration through apprenticeship of the highest
    /// level of skills and performance in industry trades and professions.
    /// This is an exact match of MasterCertificate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("MasterCertificate")]
    MasterCertificate,

    /// <summary>
    /// Credential awarded for a graduate level course of study where course work
    /// and activities advance skills beyond those of the bachelor's degree or its equivalent.
    /// Equivalent to an award at UNESCO ISCED 2011, Level 7.
    /// This is an exact match of MasterDegree in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("MasterDegree")]
    MasterDegree,

    /// <summary>
    /// Credential that addresses a subset of field-specific knowledge, skills,
    /// or competencies; often developmental with relationships to other micro-credentials
    /// and field credentials.
    /// This is an exact match of MicroCredential in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("MicroCredential")]
    MicroCredential,

    /// <summary>
    /// Doctoral degree conferred for advanced work beyond the master level, including
    /// the preparation and defense of a thesis or dissertation based on original research,
    /// or the planning and execution of an original project demonstrating substantial
    /// artistic or scholarly achievement.
    /// Equivalent to an award at UNESCO ISCED 2011, Level 8.
    /// This is an exact match of ResearchDoctorate in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("ResearchDoctorate")]
    ResearchDoctorate,

    /// <summary>
    /// Diploma awarded by secondary education institutions for successful completion
    /// of a secondary school program of study.
    /// Equivalent to an award at UNESCO ISCED 2011 Levels 2 or 3.
    /// This is an exact match of SecondarySchoolDiploma in [CTDL-TERMS].
    /// </summary>
    [JsonPropertyName("SecondarySchoolDiploma")]
    SecondarySchoolDiploma

    // Additional proprietary terms must start with 'ext:'
}
