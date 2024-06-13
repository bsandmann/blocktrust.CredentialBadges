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
    // [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "BasicOpenBadgeCredential.json", SearchOption.AllDirectories)]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "EndorsementCredential.json", SearchOption.AllDirectories)]
    public void TestRoundtripFor_OpenBadges_Examples(string documentFilename, string documentFileContents)
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