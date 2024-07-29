namespace Blocktrust.CredentialBadges.Builder.Tests.Commands.NewDid;

/// <summary>
/// Utility class for common test helper methods
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Method to generate a random hex string
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateRandomHexString(int length)
    {
        var random = new Random();
        var buffer = new byte[length / 2];
        random.NextBytes(buffer);
        return BitConverter.ToString(buffer).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Method to generate a random name
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateRandomName(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Method to generate a random API key
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateRandomApiKey(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}