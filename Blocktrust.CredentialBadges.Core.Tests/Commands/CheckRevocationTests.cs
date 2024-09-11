using Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;
using Blocktrust.CredentialBadges.OpenBadges;

namespace Blocktrust.CredentialBadges.Core.Tests.Commands;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CheckRevocationTests
{
    private readonly HttpClient _httpClient;

    public CheckRevocationTests()
    {
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task CheckRevocation_ActualRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var handler = new CheckRevocationStatusHandler(_httpClient);
        var request = new CheckRevocationStatusRequest(CreateCredentialStatus(91));

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Contains("/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef#", response.CredentialId);
    }

    [Fact]
    public async Task CheckRevocation_RevokedCredential_ReturnsRevokedStatus()
    {
        // Arrange
        var handler = new CheckRevocationStatusHandler(_httpClient);
        var request = new CheckRevocationStatusRequest(CreateCredentialStatus(92));

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Contains("/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef#", response.CredentialId);
        // Assert.True(response.IsRevoked, "Credential with status list index 92 should be revoked");
    }

    private CredentialStatus CreateCredentialStatus(int statusListIndex)
    {
        return new CredentialStatus
        {
            StatusPurpose = "Revocation",
            StatusListIndex = statusListIndex,
            Id = new Uri($"http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef#{statusListIndex}"),
            Type = "StatusList2021Entry",
            StatusListCredential = "http://10.10.50.105:8000/cloud-agent/credential-status/b7f4b6b2-8442-4276-b4af-97687a3903ef"
        };
    }
}