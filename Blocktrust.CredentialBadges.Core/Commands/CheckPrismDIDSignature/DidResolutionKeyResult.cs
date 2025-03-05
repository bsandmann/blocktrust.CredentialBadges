namespace Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;

/// <summary>
/// A small DTO/record to hold the outcome of a DID Document resolution & public key extraction.
/// This helps simplify our branching logic.
/// </summary>
public record DidResolutionKeyResult
{
    public bool IsSuccess { get; init; }
    public bool IsDeactivated { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public byte[] PublicKey { get; init; } = Array.Empty<byte>();

    // For success
    public static DidResolutionKeyResult Success(byte[] key) => new()
    {
        IsSuccess = true,
        IsDeactivated = false,
        PublicKey = key
    };

    // For generic failure (unreachable, invalid doc, etc.)
    public static DidResolutionKeyResult Failure(string error) => new()
    {
        IsSuccess = false,
        IsDeactivated = false,
        ErrorMessage = error
    };

    // For "deactivated" specifically
    public static DidResolutionKeyResult Deactivated(string error) => new()
    {
        IsSuccess = false,
        IsDeactivated = true,
        ErrorMessage = error
    };
}