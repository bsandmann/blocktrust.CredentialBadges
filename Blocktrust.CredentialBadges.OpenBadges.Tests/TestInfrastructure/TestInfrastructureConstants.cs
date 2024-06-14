namespace Blocktrust.CredentialBadges.OpenBadges.Tests.TestInfrastructure;

/// <summary>
/// Some constants and utilities used in tests.
/// </summary>
public static class TestInfrastructureConstants
{
    /// <summary>
    /// Path to test files from the OpenBadges Spec
    /// <see cref="https://www.imsglobal.org/spec/ob/v3p0/#examples-0"/>
    /// </summary>
    public const string RelativeTestPathToOpenBadges = "..//..//..//TestDocuments";
    
    /// <summary>
    /// Test material is loaded from external files. This check preconditions assumed on them hold.
    /// </summary>
    /// <param name="documentFilename">The filename under test.</param>
    /// <param name="documentFileContents">The contents of the file being tested.</param>
    public static void ThrowIfPreconditionFails(string documentFilename, string documentFileContents)
    {
        ArgumentNullException.ThrowIfNull(documentFilename);
            
        if(string.IsNullOrWhiteSpace(documentFileContents))
        {
            throw new ArgumentException($"The test file {documentFilename} must not be empty or null.", nameof(documentFileContents));
        }
    }
}