namespace Blocktrust.CredentialBadges.Core.Tests;

using Common;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using OpenBadges;
using OpenBadges.Enums;

public class CredentialParserTests
{
    [Fact]
    public void IdentusGenerated_Base64_encoded_Credential_is_parsed_successfully()
    {
        // Arrange
        // This is a Base64 encoded VC from Identus, created on 18.06.2024 for testing purposes
        // Stinctly this is not a valid OpenBadge since it is missing the required type-property,
        // as well as the @context
        var input = """
                    ZXlKaGJHY2lPaUpGVXpJMU5rc2lmUS5leUpwYzNNaU9pSmthV1E2Y0hKcGMyMDZPV016WlRRM09XUm1Oalk1WlRRellUSTNNekUxTmpVNVl6VmxaRGN4TlRRd05qSXpPREl3TTJWbU9XSTJZV00yTnpZeE1EYzJaR1F4TjJZMk9XSmtOenBEYjFGQ1EyOUZRa1ZyU1V0RWJURTFURmRzZW1NelZuQmliV04wWVRKV05VVkJTa3RNWjI5S1l6SldhbU5FU1RGT2JYTjRSV2xGUkU5SFNISmpZa3hDYkU0eldHUm5ZVlUzU0VOVmJXZzRValJVYUc4dE9FVmxZVjlrTlZsWFp6VkRibFZUVDNkdlNHSlhSbnBrUjFaNVRVSkJRbE5wTkV0RFdFNXNXVE5CZVU1VVduSk5Va2xvUVhsSFRuaFhXbXRoVDBKUGRIazNOV1Y2ZW1WM1NYRlZiRzQ0TUhaV2FGcExUREJaVUhaaFpGQTVRVGdpTENKemRXSWlPaUprYVdRNmNISnBjMjA2Tm1JeU5ETTRORFZpTm1SalkySmpOR0UwTVdVeFlXUXdNbUUxTURWaU5qZGpNRGRpT0dOaU4yUmlOekE0WlRKak9UbG1OR0kwTURkbU1ETTRNbVF4WlRwRGJuTkxaVkpKTmtObldtaGtXRkp2VEZSRlVVSkZiM1ZEWjJ4NldsZE9kMDFxVlRKaGVrVlRTVkZNWnpOak9XbzNMVWhTV0VWRVpHVlZkbWh3VFhWYVdtdE5ZVEpDVEZNMFNqbFVWSE5SWnpJMVJWRk9Va2szUTJka2RGbFlUakJhV0VsM1JVRkdTMHhuYjBwak1sWnFZMFJKTVU1dGMzaEZhVVZFYVU4eFF6YzFTMVpMZG1wWmMzWjBaelZNVFRaSmNrOUJaVmN0V1hrMFExODRUelJvWm04NVJtODRaeUlzSW01aVppSTZNVGN4T0RVME5Ua3dOQ3dpZG1NaU9uc2lZM0psWkdWdWRHbGhiRk4xWW1wbFkzUWlPbnNpWVdOb2FXVjJaVzFsYm5RaU9uc2lZV05vYVdWMlpXMWxiblJVZVhCbElqb2lSR2x3Ykc5dFlTSXNJbWx0WVdkbElqcDdJbWxrSWpvaWFIUjBjSE02WEM5Y0wzVnpaWEl0YVcxaFoyVnpMbWRwZEdoMVluVnpaWEpqYjI1MFpXNTBMbU52YlZ3dk56VXlNekkyWEM4eU1UUTVORGMzTVRNdE1UVTRNalpoTTJFdFlqVmhZeTAwWm1KaExUaGtOR0V0T0RnMFlqWXdZMkkzTVRVM0xuQnVaeUlzSW5SNWNHVWlPaUpKYldGblpTSjlMQ0pqY21sMFpYSnBZU0k2ZXlKdVlYSnlZWFJwZG1VaU9pSlVhR2x6SUdOeVpXUmxiblJwWVd3Z2QyRnpJR2x6YzNWbFpDQjBieUJoSUhOMGRXUmxiblFnZEdoaGRDQmtaVzF2Ym5OMGNtRjBaV1FnY0hKdlptbGphV1Z1WTNrZ2FXNGdkR2hsSUZCNWRHaHZiaUJ3Y205bmNtRnRiV2x1WnlCc1lXNW5kV0ZuWlNCMGFISnZkV2RvSUdGamRHbDJhWFJwWlhNZ2NHVnlabTl5YldWa0lHbHVJSFJvWlNCamIzVnljMlVnZEdsMGJHVmtJQ3BKYm5SeWIyUjFZM1JwYjI0Z2RHOGdVSGwwYUc5dUtpQnZabVpsY21Wa0lHSjVJRnRGZUdGdGNHeGxJRWx1YzNScGRIVjBaU0J2WmlCVVpXTm9ibTlzYjJkNVhTaG9kSFJ3Y3pwY0wxd3ZaWGhwZEM1bGVHRnRjR3hsTG1Wa2RTa2dabkp2YlNBcUtrWmxZbkoxWVhKNUlERTNMQ0F5TURJektpb2dkRzhnS2lwS2RXNWxJREV5TENBeU1ESXpLaW91SUZSb2FYTWdhWE1nWVNCamNtVmtaVzUwYVdGc0lIZHBkR2dnZEdobElHWnZiR3h2ZDJsdVp5QmpjbWwwWlhKcFlUcGNiakV1SUdOdmJYQnNaWFJsWkNCaGJHd2dhRzl0WlhkdmNtc2dZWE56YVdkdWJXVnVkSE5jYmpJdUlIQmhjM05sWkNCaGJHd2daWGhoYlhOY2JqTXVJR052YlhCc1pYUmxaQ0JtYVc1aGJDQm5jbTkxY0NCd2NtOXFaV04wSWl3aWRIbHdaU0k2SWtOeWFYUmxjbWxoSW4wc0ltNWhiV1VpT2lKQ1lXUm5aU0lzSW1SbGMyTnlhWEIwYVc5dUlqb2lWR2hwY3lCcGN5QmhJSE5oYlhCc1pTQmpjbVZrWlc1MGFXRnNJR2x6YzNWbFpDQmllU0IwYUdVZ1JHbG5hWFJoYkNCRGNtVmtaVzUwYVdGc2N5QkRiMjV6YjNKMGFYVnRJSFJ2SUdSbGJXOXVjM1J5WVhSbElIUm9aU0JtZFc1amRHbHZibUZzYVhSNUlHOW1JRlpsY21sbWFXRmliR1VnUTNKbFpHVnVkR2xoYkhNZ1ptOXlJSGRoYkd4bGRITWdZVzVrSUhabGNtbG1hV1Z5Y3k0aUxDSnBaQ0k2SW5WeWJqcDFkV2xrT21Ka05tUTVNekUyTFdZM1lXVXROREEzTXkxaE1XVTFMVEptTjJZMVltUXlNamt5TWlJc0luUjVjR1VpT2xzaVFXTm9hV1YyWlcxbGJuUWlYWDBzSW1sa0lqb2laR2xrT25CeWFYTnRPalppTWpRek9EUTFZalprWTJOaVl6UmhOREZsTVdGa01ESmhOVEExWWpZM1l6QTNZamhqWWpka1lqY3dPR1V5WXprNVpqUmlOREEzWmpBek9ESmtNV1U2UTI1elMyVlNTVFpEWjFwb1pGaFNiMHhVUlZGQ1JXOTFRMmRzZWxwWFRuZE5hbFV5WVhwRlUwbFJUR2N6WXpscU55MUlVbGhGUkdSbFZYWm9jRTExV2xwclRXRXlRa3hUTkVvNVZGUnpVV2N5TlVWUlRsSkpOME5uWkhSWldFNHdXbGhKZDBWQlJrdE1aMjlLWXpKV2FtTkVTVEZPYlhONFJXbEZSR2xQTVVNM05VdFdTM1pxV1hOMmRHYzFURTAyU1hKUFFXVlhMVmw1TkVOZk9FODBhR1p2T1Vadk9HY2lMQ0owZVhCbElqcGJJa0ZqYUdsbGRtVnRaVzUwVTNWaWFtVmpkQ0pkZlN3aWRIbHdaU0k2V3lKV1pYSnBabWxoWW14bFEzSmxaR1Z1ZEdsaGJDSmRMQ0pBWTI5dWRHVjRkQ0k2V3lKb2RIUndjenBjTDF3dmQzZDNMbmN6TG05eVoxd3ZNakF4T0Z3dlkzSmxaR1Z1ZEdsaGJITmNMM1l4SWwwc0ltTnlaV1JsYm5ScFlXeFRkR0YwZFhNaU9uc2ljM1JoZEhWelVIVnljRzl6WlNJNklsSmxkbTlqWVhScGIyNGlMQ0p6ZEdGMGRYTk1hWE4wU1c1a1pYZ2lPak1zSW1sa0lqb2lhSFIwY0RwY0wxd3ZNVEF1TVRBdU5UQXVNVEExT2pnd01EQmNMMk5zYjNWa0xXRm5aVzUwWEM5amNtVmtaVzUwYVdGc0xYTjBZWFIxYzF3dllqZG1OR0kyWWpJdE9EUTBNaTAwTWpjMkxXSTBZV1l0T1RjMk9EZGhNemt3TTJWbUl6TWlMQ0owZVhCbElqb2lVM1JoZEhWelRHbHpkREl3TWpGRmJuUnllU0lzSW5OMFlYUjFjMHhwYzNSRGNtVmtaVzUwYVdGc0lqb2lhSFIwY0RwY0wxd3ZNVEF1TVRBdU5UQXVNVEExT2pnd01EQmNMMk5zYjNWa0xXRm5aVzUwWEM5amNtVmtaVzUwYVdGc0xYTjBZWFIxYzF3dllqZG1OR0kyWWpJdE9EUTBNaTAwTWpjMkxXSTBZV1l0T1RjMk9EZGhNemt3TTJWbUluMTlmUS5GWGNoUkp5azgzZ25NLWpFcHlGQzFEVkgzYUYwZ2NfQ054Z2ZwMi1HTlpMWFZwSno2Q21pSnJlYldiUU1Vak9JQ241QzlRRUpRZk85dlM5em4teU9YUQ==
                    """;

        // Act
        var result = CredentialParser.Parse(input);
        
        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType(typeof(AchievementCredential));
        result.Value.Should().NotBeOfType(typeof(EndorsementCredential));
        var achievementCredential = (AchievementCredential)result.Value;
        
        achievementCredential.Type.Should().Contain("VerifiableCredential");
        achievementCredential.Context.Should().Contain(new Uri("https://www.w3.org/2018/credentials/v1"));
        achievementCredential.Issuer.Id.Should().Be("did:prism:9c3e479df669e43a27315659c5ed715406238203ef9b6ac6761076dd17f69bd7:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiEDOGHrcbLBlN3XdgaU7HCUmh8R4Tho-8Eea_d5YWg5CnUSOwoHbWFzdGVyMBABSi4KCXNlY3AyNTZrMRIhAyGNxWZkaOBOty75ezzewIqUln80vVhZKL0YPvadP9A8");
        achievementCredential.ValidFrom.Should().Be(DateTimeOffset.FromUnixTimeSeconds(long.Parse("1718545904")).DateTime);
        achievementCredential.ValidUntil.Should().BeNull();
        
        achievementCredential.CredentialStatus.Should().NotBeNull();
        achievementCredential.CredentialStatus.Id.Should().Be("http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef#3");
        achievementCredential.CredentialStatus.Type.Should().Contain("StatusList2021Entry");
        achievementCredential.CredentialStatus.StatusListCredential.Should().Be("http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef");
        achievementCredential.CredentialStatus.StatusPurpose.Should().Be("Revocation");
        achievementCredential.CredentialStatus.StatusListIndex.Should().Be(3);

        achievementCredential.CredentialSubject.Id.Should().Be("did:prism:6b243845b6dccbc4a41e1ad02a505b67c07b8cb7db708e2c99f4b407f0382d1e:CnsKeRI6CgZhdXRoLTEQBEouCglzZWNwMjU2azESIQLg3c9j7-HRXEDdeUvhpMuZZkMa2BLS4J9TTsQg25EQNRI7CgdtYXN0ZXIwEAFKLgoJc2VjcDI1NmsxEiEDiO1C75KVKvjYsvtg5LM6IrOAeW-Yy4C_8O4hfo9Fo8g");
        achievementCredential.CredentialSubject.Type.Should().Contain("AchievementSubject");
        achievementCredential.CredentialSubject.Achievement.AchievementType.Should().Be(EAchievementType.Diploma);
        achievementCredential.CredentialSubject.Achievement.Image.Id.Should().Be("https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png");
        achievementCredential.CredentialSubject.Achievement.Image.Type.Should().Be("Image");
        achievementCredential.CredentialSubject.Achievement.Criteria.Narrative.Should().Be("This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project");
        achievementCredential.CredentialSubject.Achievement.Criteria.Type.Should().Be("Criteria");
        achievementCredential.CredentialSubject.Achievement.Name.Should().Be("Badge");
        achievementCredential.CredentialSubject.Achievement.Description.Should().Be("This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.");
        achievementCredential.CredentialSubject.Achievement.Id.Should().Be("urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922");
        achievementCredential.CredentialSubject.Achievement.Type.Should().Contain("Achievement");

        achievementCredential.Jwt.Headers.Should().ContainKey("alg");
        achievementCredential.Jwt.Headers["alg"].ToString().Should().Be("ES256K");
        achievementCredential.Jwt.Signature.Should().Be("FXchRJyk83gnM-jEpyFC1DVH3aF0gc_CNxgfp2-GNZLXVpJz6CmiJrebWbQMUjOICn5C9QEJQfO9vS9zn-yOXQ");
    }
    
     [Fact]
    public void IdentusGenerated_JWT_encoded_Credential_is_parsed_successfully()
    {
        // Arrange
        // This is a JWT encoded VC from Identus, created on 18.06.2024 for testing purposes
        // Stinctly this is not a valid OpenBadge since it is missing the required type-property,
        // as well as the @context
        var input = """
                    eyJhbGciOiJFUzI1NksifQ.eyJpc3MiOiJkaWQ6cHJpc206OWMzZTQ3OWRmNjY5ZTQzYTI3MzE1NjU5YzVlZDcxNTQwNjIzODIwM2VmOWI2YWM2NzYxMDc2ZGQxN2Y2OWJkNzpDb1FCQ29FQkVrSUtEbTE1TFdsemMzVnBibWN0YTJWNUVBSktMZ29KYzJWamNESTFObXN4RWlFRE9HSHJjYkxCbE4zWGRnYVU3SENVbWg4UjRUaG8tOEVlYV9kNVlXZzVDblVTT3dvSGJXRnpkR1Z5TUJBQlNpNEtDWE5sWTNBeU5UWnJNUkloQXlHTnhXWmthT0JPdHk3NWV6emV3SXFVbG44MHZWaFpLTDBZUHZhZFA5QTgiLCJzdWIiOiJkaWQ6cHJpc206NmIyNDM4NDViNmRjY2JjNGE0MWUxYWQwMmE1MDViNjdjMDdiOGNiN2RiNzA4ZTJjOTlmNGI0MDdmMDM4MmQxZTpDbnNLZVJJNkNnWmhkWFJvTFRFUUJFb3VDZ2x6WldOd01qVTJhekVTSVFMZzNjOWo3LUhSWEVEZGVVdmhwTXVaWmtNYTJCTFM0SjlUVHNRZzI1RVFOUkk3Q2dkdFlYTjBaWEl3RUFGS0xnb0pjMlZqY0RJMU5tc3hFaUVEaU8xQzc1S1ZLdmpZc3Z0ZzVMTTZJck9BZVctWXk0Q184TzRoZm85Rm84ZyIsIm5iZiI6MTcxODU0NTkwNCwidmMiOnsiY3JlZGVudGlhbFN1YmplY3QiOnsiYWNoaWV2ZW1lbnQiOnsiYWNoaWV2ZW1lbnRUeXBlIjoiRGlwbG9tYSIsImltYWdlIjp7ImlkIjoiaHR0cHM6XC9cL3VzZXItaW1hZ2VzLmdpdGh1YnVzZXJjb250ZW50LmNvbVwvNzUyMzI2XC8yMTQ5NDc3MTMtMTU4MjZhM2EtYjVhYy00ZmJhLThkNGEtODg0YjYwY2I3MTU3LnBuZyIsInR5cGUiOiJJbWFnZSJ9LCJjcml0ZXJpYSI6eyJuYXJyYXRpdmUiOiJUaGlzIGNyZWRlbnRpYWwgd2FzIGlzc3VlZCB0byBhIHN0dWRlbnQgdGhhdCBkZW1vbnN0cmF0ZWQgcHJvZmljaWVuY3kgaW4gdGhlIFB5dGhvbiBwcm9ncmFtbWluZyBsYW5ndWFnZSB0aHJvdWdoIGFjdGl2aXRpZXMgcGVyZm9ybWVkIGluIHRoZSBjb3Vyc2UgdGl0bGVkICpJbnRyb2R1Y3Rpb24gdG8gUHl0aG9uKiBvZmZlcmVkIGJ5IFtFeGFtcGxlIEluc3RpdHV0ZSBvZiBUZWNobm9sb2d5XShodHRwczpcL1wvZXhpdC5leGFtcGxlLmVkdSkgZnJvbSAqKkZlYnJ1YXJ5IDE3LCAyMDIzKiogdG8gKipKdW5lIDEyLCAyMDIzKiouIFRoaXMgaXMgYSBjcmVkZW50aWFsIHdpdGggdGhlIGZvbGxvd2luZyBjcml0ZXJpYTpcbjEuIGNvbXBsZXRlZCBhbGwgaG9tZXdvcmsgYXNzaWdubWVudHNcbjIuIHBhc3NlZCBhbGwgZXhhbXNcbjMuIGNvbXBsZXRlZCBmaW5hbCBncm91cCBwcm9qZWN0IiwidHlwZSI6IkNyaXRlcmlhIn0sIm5hbWUiOiJCYWRnZSIsImRlc2NyaXB0aW9uIjoiVGhpcyBpcyBhIHNhbXBsZSBjcmVkZW50aWFsIGlzc3VlZCBieSB0aGUgRGlnaXRhbCBDcmVkZW50aWFscyBDb25zb3J0aXVtIHRvIGRlbW9uc3RyYXRlIHRoZSBmdW5jdGlvbmFsaXR5IG9mIFZlcmlmaWFibGUgQ3JlZGVudGlhbHMgZm9yIHdhbGxldHMgYW5kIHZlcmlmaWVycy4iLCJpZCI6InVybjp1dWlkOmJkNmQ5MzE2LWY3YWUtNDA3My1hMWU1LTJmN2Y1YmQyMjkyMiIsInR5cGUiOlsiQWNoaWV2ZW1lbnQiXX0sImlkIjoiZGlkOnByaXNtOjZiMjQzODQ1YjZkY2NiYzRhNDFlMWFkMDJhNTA1YjY3YzA3YjhjYjdkYjcwOGUyYzk5ZjRiNDA3ZjAzODJkMWU6Q25zS2VSSTZDZ1poZFhSb0xURVFCRW91Q2dselpXTndNalUyYXpFU0lRTGczYzlqNy1IUlhFRGRlVXZocE11WlprTWEyQkxTNEo5VFRzUWcyNUVRTlJJN0NnZHRZWE4wWlhJd0VBRktMZ29KYzJWamNESTFObXN4RWlFRGlPMUM3NUtWS3ZqWXN2dGc1TE02SXJPQWVXLVl5NENfOE80aGZvOUZvOGciLCJ0eXBlIjpbIkFjaGlldmVtZW50U3ViamVjdCJdfSwidHlwZSI6WyJWZXJpZmlhYmxlQ3JlZGVudGlhbCJdLCJAY29udGV4dCI6WyJodHRwczpcL1wvd3d3LnczLm9yZ1wvMjAxOFwvY3JlZGVudGlhbHNcL3YxIl0sImNyZWRlbnRpYWxTdGF0dXMiOnsic3RhdHVzUHVycG9zZSI6IlJldm9jYXRpb24iLCJzdGF0dXNMaXN0SW5kZXgiOjMsImlkIjoiaHR0cDpcL1wvMTAuMTAuNTAuMTA1OjgwMDBcL2Nsb3VkLWFnZW50XC9jcmVkZW50aWFsLXN0YXR1c1wvYjdmNGI2YjItODQ0Mi00Mjc2LWI0YWYtOTc2ODdhMzkwM2VmIzMiLCJ0eXBlIjoiU3RhdHVzTGlzdDIwMjFFbnRyeSIsInN0YXR1c0xpc3RDcmVkZW50aWFsIjoiaHR0cDpcL1wvMTAuMTAuNTAuMTA1OjgwMDBcL2Nsb3VkLWFnZW50XC9jcmVkZW50aWFsLXN0YXR1c1wvYjdmNGI2YjItODQ0Mi00Mjc2LWI0YWYtOTc2ODdhMzkwM2VmIn19fQ.FXchRJyk83gnM-jEpyFC1DVH3aF0gc_CNxgfp2-GNZLXVpJz6CmiJrebWbQMUjOICn5C9QEJQfO9vS9zn-yOXQ
                    """;

        // Act
        var result = CredentialParser.Parse(input);
        
        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeOfType(typeof(AchievementCredential));
        result.Value.Should().NotBeOfType(typeof(EndorsementCredential));
        var achievementCredential = (AchievementCredential)result.Value;
        
        achievementCredential.Type.Should().Contain("VerifiableCredential");
        achievementCredential.Context.Should().Contain(new Uri("https://www.w3.org/2018/credentials/v1"));
        achievementCredential.Issuer.Id.Should().Be("did:prism:9c3e479df669e43a27315659c5ed715406238203ef9b6ac6761076dd17f69bd7:CoQBCoEBEkIKDm15LWlzc3Vpbmcta2V5EAJKLgoJc2VjcDI1NmsxEiEDOGHrcbLBlN3XdgaU7HCUmh8R4Tho-8Eea_d5YWg5CnUSOwoHbWFzdGVyMBABSi4KCXNlY3AyNTZrMRIhAyGNxWZkaOBOty75ezzewIqUln80vVhZKL0YPvadP9A8");
        achievementCredential.ValidFrom.Should().Be(DateTimeOffset.FromUnixTimeSeconds(long.Parse("1718545904")).DateTime);
        achievementCredential.ValidUntil.Should().BeNull();
        
        achievementCredential.CredentialStatus.Should().NotBeNull();
        achievementCredential.CredentialStatus.Id.Should().Be("http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef#3");
        achievementCredential.CredentialStatus.Type.Should().Contain("StatusList2021Entry");
        achievementCredential.CredentialStatus.StatusListCredential.Should().Be("http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef");
        achievementCredential.CredentialStatus.StatusPurpose.Should().Be("Revocation");
        achievementCredential.CredentialStatus.StatusListIndex.Should().Be(3);

        achievementCredential.CredentialSubject.Id.Should().Be("did:prism:6b243845b6dccbc4a41e1ad02a505b67c07b8cb7db708e2c99f4b407f0382d1e:CnsKeRI6CgZhdXRoLTEQBEouCglzZWNwMjU2azESIQLg3c9j7-HRXEDdeUvhpMuZZkMa2BLS4J9TTsQg25EQNRI7CgdtYXN0ZXIwEAFKLgoJc2VjcDI1NmsxEiEDiO1C75KVKvjYsvtg5LM6IrOAeW-Yy4C_8O4hfo9Fo8g");
        achievementCredential.CredentialSubject.Type.Should().Contain("AchievementSubject");
        achievementCredential.CredentialSubject.Achievement.AchievementType.Should().Be(EAchievementType.Diploma);
        achievementCredential.CredentialSubject.Achievement.Image.Id.Should().Be("https://user-images.githubusercontent.com/752326/214947713-15826a3a-b5ac-4fba-8d4a-884b60cb7157.png");
        achievementCredential.CredentialSubject.Achievement.Image.Type.Should().Be("Image");
        achievementCredential.CredentialSubject.Achievement.Criteria.Narrative.Should().Be("This credential was issued to a student that demonstrated proficiency in the Python programming language through activities performed in the course titled *Introduction to Python* offered by [Example Institute of Technology](https://exit.example.edu) from **February 17, 2023** to **June 12, 2023**. This is a credential with the following criteria:\n1. completed all homework assignments\n2. passed all exams\n3. completed final group project");
        achievementCredential.CredentialSubject.Achievement.Criteria.Type.Should().Be("Criteria");
        achievementCredential.CredentialSubject.Achievement.Name.Should().Be("Badge");
        achievementCredential.CredentialSubject.Achievement.Description.Should().Be("This is a sample credential issued by the Digital Credentials Consortium to demonstrate the functionality of Verifiable Credentials for wallets and verifiers.");
        achievementCredential.CredentialSubject.Achievement.Id.Should().Be("urn:uuid:bd6d9316-f7ae-4073-a1e5-2f7f5bd22922");
        achievementCredential.CredentialSubject.Achievement.Type.Should().Contain("Achievement");
        
        achievementCredential.Jwt.Headers.Should().ContainKey("alg");
        achievementCredential.Jwt.Headers["alg"].ToString().Should().Be("ES256K");
        achievementCredential.Jwt.Signature.Should().Be("FXchRJyk83gnM-jEpyFC1DVH3aF0gc_CNxgfp2-GNZLXVpJz6CmiJrebWbQMUjOICn5C9QEJQfO9vS9zn-yOXQ");
    }
}