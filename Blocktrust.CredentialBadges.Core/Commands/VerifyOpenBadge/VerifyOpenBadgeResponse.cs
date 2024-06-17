namespace Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;

/// <summary>
/// The results should include all the checks that will be displayed on the verifer-website
/// A good initial reference is the <see cref="https://verifierplus.org/"/> website of the DCC.
/// Here is a example credentials one can try: <see cref="https://github.com/digitalcredentials/docs/blob/main/example-credentials/criteria-markdown-bold/credential.json"/>
/// More Credentials can be found in the other folders of the docs in GitHub.
/// </summary>
public class VerifyOpenBadgeResponse
{
    public DateTime CheckedOn { get; set; }

    public bool VerificationIsSuccessfull()
    {
        var isSuccessfull = true;
        if (SignatureIsValid == false)
        {
            isSuccessfull = false;
        }

        if (IssuerFoundInTrustRegistry == false)
        {
            isSuccessfull = false;
        }

        if (CredentialIsNotExpired == false)
        {
            isSuccessfull = false;
        }
        
        if (CredentialIssuanceDateIsNotInFuture == false)
        {
            isSuccessfull = false;
        }
        
        if (CredentialIsNotRevoked == false)
        {
            isSuccessfull = false;
        }

        return isSuccessfull;
    }

    /// <summary>
    /// Could be a JWT or embedded proof
    /// </summary>
    public bool SignatureIsValid { get; set; }

    public bool? IssuerFoundInTrustRegistry { get; set; }
    public string? ReferenceToTrustRegistry { get; set; }


    /// <summary>
    /// Check that the credential is not expired. Note, that the expiration date is optional.
    /// </summary>
    public bool? CredentialIsNotExpired { get; set; }

    /// <summary>
    /// Check that the issuance date is not in the future
    /// </summary>
    public bool CredentialIssuanceDateIsNotInFuture { get; set; }

    /// <summary>
    /// TODO null means that the revocation status could not be checked?
    /// </summary>
    public bool? CredentialIsNotRevoked { get; set; }

    public string? ReferenceToRevocationList { get; set; }
    // Maybe additional information on the revocation list used, ...
}