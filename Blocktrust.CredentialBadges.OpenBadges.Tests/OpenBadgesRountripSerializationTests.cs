namespace Blocktrust.CredentialBadges.OpenBadges.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using TestInfrastructure;

public class OpenBadges_Tests
{
    /// <summary>
    /// This checks <see cref="AchievementCredential"/> deserialization and serialization
    /// </summary>
    [Theory]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "BasicOpenBadgeCredential.json", SearchOption.AllDirectories)]
    public void TestRoundtripFor_OpenBadges_AchievementCredential_Examples(string documentFilename, string documentFileContents)
    {
        TestInfrastructureConstants.ThrowIfPreconditionFails(documentFilename, documentFileContents);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        var (deserializedCredential, reserializedCredential) = JsonTestingUtilities.PerformSerializationCycle<AchievementCredential>(documentFileContents, options);
        
        bool areJsonElementsEqual = JsonTestingUtilities.CompareJsonElements(documentFileContents, reserializedCredential);
        Assert.True(areJsonElementsEqual, $"File \"{documentFilename}\" did not pass roundtrip test.");
    }
    
    /// <summary>
    /// This checks <see cref="AchievementCredential"/> deserialization and serialization
    /// </summary>
    [Theory]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "AchievementAlignment.json", SearchOption.AllDirectories)]
    public void TestRoundtripFor_OpenBadges_AchievementAlignment_Examples(string documentFilename, string documentFileContents)
    {
        TestInfrastructureConstants.ThrowIfPreconditionFails(documentFilename, documentFileContents);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        var (deserializedCredential, reserializedCredential) = JsonTestingUtilities.PerformSerializationCycle<AchievementCredential>(documentFileContents, options);
        
        bool areJsonElementsEqual = JsonTestingUtilities.CompareJsonElements(documentFileContents, reserializedCredential);
        Assert.True(areJsonElementsEqual, $"File \"{documentFilename}\" did not pass roundtrip test.");
    }
    
    /// <summary>
    /// This checks <see cref="AchievementCredential"/> deserialization and serialization
    /// </summary>
    [Theory]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "SkillAssertion.json", SearchOption.AllDirectories)]
    public void TestRoundtripFor_OpenBadges_SkillAssertion_Examples(string documentFilename, string documentFileContents)
    {
        TestInfrastructureConstants.ThrowIfPreconditionFails(documentFilename, documentFileContents);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        var (deserializedCredential, reserializedCredential) = JsonTestingUtilities.PerformSerializationCycle<AchievementCredential>(documentFileContents, options);
        
        bool areJsonElementsEqual = JsonTestingUtilities.CompareJsonElements(documentFileContents, reserializedCredential);
        Assert.True(areJsonElementsEqual, $"File \"{documentFilename}\" did not pass roundtrip test.");
    }
    
    /// <summary>
    /// This checks <see cref="EndorsementCredential"/> deserialization and serialization
    /// </summary>
    [Theory]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "EndorsementCredential.json", SearchOption.AllDirectories)]
    public void TestRoundtripFor_OpenBadges_EndorsementCredential_Examples(string documentFilename, string documentFileContents)
    {
        TestInfrastructureConstants.ThrowIfPreconditionFails(documentFilename, documentFileContents);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        var (deserializedCredential, reserializedCredential) = JsonTestingUtilities.PerformSerializationCycle<EndorsementCredential>(documentFileContents, options);
        
        bool areJsonElementsEqual = JsonTestingUtilities.CompareJsonElements(documentFileContents, reserializedCredential);
        Assert.True(areJsonElementsEqual, $"File \"{documentFilename}\" did not pass roundtrip test.");
    }
}