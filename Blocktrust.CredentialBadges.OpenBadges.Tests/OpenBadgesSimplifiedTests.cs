namespace Blocktrust.CredentialBadges.OpenBadges.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using TestInfrastructure;

// These tests here do not try to attempt to make a full rountrip, since many smaller issues
// We currently don't need to deal with, like ordering, or replaying a "42.0" exactly back as "42.0"
// instead of "42". We just want to make sure that the deserialization works,

public class OpenBadgesSimplifiedTests
{
    /// <summary>
    /// This checks <see cref="AchievementCredential"/> deserialization and serialization
    /// </summary>
    [Theory]
    [FilesData(TestInfrastructureConstants.RelativeTestPathToOpenBadges, "CompleteOpenBadgeCredential.json", SearchOption.AllDirectories)]
    public void TestSuccessfull_Deserialization_for_CompleteOpenBadgeCredential(string documentFilename, string documentFileContents)
    {
        TestInfrastructureConstants.ThrowIfPreconditionFails(documentFilename, documentFileContents);

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        
        var (deserializedCredential, reserializedCredential) = JsonTestingUtilities.PerformSerializationCycle<AchievementCredential>(documentFileContents, options);
        
        Assert.True(!string.IsNullOrEmpty(reserializedCredential));
    }
}